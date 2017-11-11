﻿using Dapper;
using Microsoft.EntityFrameworkCore;
using ProjectIvy.BL.MapExtensions;
using ProjectIvy.DL.Databases.Main.Queries;
using ProjectIvy.DL.Extensions.Entities;
using ProjectIvy.DL.Extensions;
using ProjectIvy.DL.Sql;
using ProjectIvy.Model.Binding.Common;
using ProjectIvy.Model.Binding.Expense;
using ProjectIvy.Model.View;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System;
using View = ProjectIvy.Model.View.Expense;

namespace ProjectIvy.BL.Handlers.Expense
{
    public class ExpenseHandler : Handler<ExpenseHandler>, IExpenseHandler
    {
        public ExpenseHandler(IHandlerContext<ExpenseHandler> context) : base(context)
        {
        }

        public int Count(FilteredBinding binding)
        {
            using (var db = GetMainContext())
            {
                var expenses = db.Expenses.WhereUser(User.Id);

                expenses = binding.From.HasValue ? expenses.Where(x => x.Date >= binding.From) : expenses;
                expenses = binding.To.HasValue ? expenses.Where(x => x.Date <= binding.To) : expenses;

                return expenses.Count();
            }
        }

        public string Create(ExpenseBinding binding)
        {
            using (var db = GetMainContext())
            {
                var entity = binding.ToEntity(db);
                entity.UserId = User.Id;
                entity.ValueId = db.Expenses.NextValueId(User.Id).ToString();

                db.Expenses.Add(entity);
                db.SaveChanges();

                return entity.ValueId;
            }
        }

        public bool Delete(string valueId)
        {
            using (var db = GetMainContext())
            {
                var entity = db.Expenses.WhereUser(User.Id)
                                        .SingleOrDefault(x => x.ValueId == valueId);

                db.Expenses.Remove(entity);
                db.SaveChanges();

                return true;
            }
        }

        public View.Expense Get(string expenseId)
        {
            using (var context = GetMainContext())
            {
                var expense = context.Expenses.Include(x => x.ExpenseType)
                                              .Include(x => x.Currency)
                                              .Include(x => x.Poi)
                                              .Include(x => x.Vendor)
                                              .WhereUser(User)
                                              .SingleOrDefault(x => x.ValueId == expenseId);

                return new View.Expense(expense);
            }
        }

        public PagedView<View.Expense> Get(ExpenseGetBinding binding)
        {
            using (var context = GetMainContext())
            {
                var result = context.Expenses.Include(x => x.ExpenseType)
                                             .Include(x => x.Currency)
                                             .Include(x => x.ParentCurrency)
                                             .Include(x => x.Poi)
                                             .Include(x => x.Vendor)
                                             .Include(x => x.PaymentType)
                                             .Include(x => x.Card)
                                             .Include(x => x.ExpenseFiles)
                                             .Include($"{nameof(Model.Database.Main.Finance.Expense.ExpenseFiles)}.{nameof(Model.Database.Main.Finance.ExpenseFile.File)}.{nameof(Model.Database.Main.Storage.File.FileType)}")
                                             .Include($"{nameof(Model.Database.Main.Finance.Expense.ExpenseFiles)}.{nameof(Model.Database.Main.Finance.ExpenseFile.ExpenseFileType)}")
                                             .WhereUser(User.Id)
                                             .Where(binding, context);

                var view = new PagedView<View.Expense>();
                view.Count = result.Count();

                result = result.OrderBy(binding)
                               .Page(binding.Page, binding.PageSize);

                view.Items = result.ToList().Select(x => new View.Expense(x));

                return view;
            }
        }

        public async Task<IEnumerable<GroupedByMonth<decimal>>> GetGroupedByMonthSum(ExpenseSumGetBinding binding)
        {
            using (var context = GetMainContext())
            {
                var startDate = context.Expenses.WhereUser(User.Id).OrderBy(x => x.Date).FirstOrDefault().Date;
                var endDate = DateTime.Now;

                var periods = new List<FilteredBinding>();

                foreach (var year in Enumerable.Range(startDate.Year, endDate.Year - startDate.Year + 1))
                {
                    int startMonth = startDate.Year == year ? startDate.Month : 1;
                    int endMonth = endDate.Year == year ? endDate.Month : 12;

                    foreach (var month in Enumerable.Range(startMonth, endMonth - startMonth + 1))
                    {
                        var startOfMonth = new DateTime(year, month, 1);
                        var endOfMonth = new DateTime(year, month, DateTime.DaysInMonth(year, month));

                        periods.Add(new FilteredBinding(startOfMonth, endOfMonth));
                    }
                }

                var tasks = periods.Select(x => new KeyValuePair<FilteredBinding, Task<decimal>>(x, GetSum(binding.Override(x))));

                await System.Threading.Tasks.Task.WhenAll(tasks.Select(x => x.Value));

                return tasks.Select(x => new GroupedByMonth<decimal>(x.Value.Result, x.Key.From.Value.Year, x.Key.From.Value.Month));
            }
        }

        public async Task<IEnumerable<GroupedByYear<decimal>>> GetGroupedByYearSum(ExpenseSumGetBinding binding)
        {
            using (var context = GetMainContext())
            {
                int startYear = context.Expenses.WhereUser(User.Id).OrderBy(x => x.Date).FirstOrDefault().Date.Year;
                int endYear = DateTime.Now.Year;

                var years = Enumerable.Range(startYear, endYear - startYear + 1);

                var periods = years.Select(x => new FilteredBinding(new DateTime(x, 1, 1), new DateTime(x, 12, 31)));

                var tasks = periods.Select(x => new KeyValuePair<int, Task<decimal>>(x.From.Value.Year, GetSum(binding.Override(x))));

                await System.Threading.Tasks.Task.WhenAll(tasks.Select(x => x.Value));

                return tasks.Select(x => new GroupedByYear<decimal>(x.Value.Result, x.Key));
            }
        }

        public async Task<IEnumerable<KeyValuePair<string, decimal>>> GetGroupedByTypeSum(ExpenseSumGetBinding binding)
        {
            using (var context = GetMainContext())
            {
                var result = context.Expenses.Include(x => x.ExpenseType)
                                             .WhereUser(User.Id);

                result = binding.From.HasValue ? result.Where(x => x.Date >= binding.From) : result;
                result = binding.To.HasValue ? result.Where(x => x.Date <= binding.To) : result;

                var types = result.Select(x => x.ExpenseType.ValueId).Distinct().ToList();

                var tasks = types.Select(x => new KeyValuePair<string, Task<decimal>>(x, GetSum(binding.Override(x))));

                await System.Threading.Tasks.Task.WhenAll(tasks.Select(x => x.Value));

                return tasks.Select(x => new KeyValuePair<string, decimal>(x.Key, x.Value.Result))
                            .OrderByDescending(x => x.Value);
            }
        }

        public async Task<decimal> GetSum(ExpenseSumGetBinding binding)
        {
            using (var context = GetMainContext())
            {
                int targetCurrencyId = context.GetCurrencyId(binding.TargetCurrencyId, User.Id);

                var expenseIds = context.Expenses.WhereUser(User)
                                                 .Where(binding, context)
                                                 .Select(x => x.Id)
                                                 .ToList();

                if (!expenseIds.Any())
                    return 0;

                using (var sql = GetSqlConnection())
                {
                    var parameters = new GetExpenseSumQuery()
                    {
                        ExpenseIds = expenseIds,
                        TargetCurrencyId = targetCurrencyId,
                        UserId = User.Id
                    };
                    return Math.Round(await sql.ExecuteScalarAsync<decimal>(SqlLoader.Load(MainSnippets.GetExpenseSumInDefaultCurrency), parameters), 2);
                }
            }
        }

        public bool Update(ExpenseBinding binding)
        {
            using (var context = GetMainContext())
            {
                var entity = context.Expenses.WhereUser(User.Id).SingleOrDefault(x => x.ValueId == binding.Id);

                entity = binding.ToEntity(context, entity);

                context.Expenses.Update(entity);
                context.SaveChanges();

                return true;
            }
        }
    }
}

using Microsoft.EntityFrameworkCore;
using ProjectIvy.Data.DbContexts;
using ProjectIvy.Data.Extensions.Entities;
using ProjectIvy.Model.Database.Main.Finance;

namespace ProjectIvy.Data.Test.Extensions;

public class ExpenseTransactionTests
{
    [Fact]
    public void ExpenseProjectsMultipleTransactionsFromTwoColumnLinks()
    {
        using var db = new MainContext("Server=localhost;Database=ModelOnly;Integrated Security=true");
        var expense = new Expense { Id = 1, UserId = 2 };
        var first = new Transaction { Id = 3 };
        var second = new Transaction { Id = 4 };

        db.ExpenseTransactions.AddRange(
            new ExpenseTransaction { Expense = expense, Transaction = first },
            new ExpenseTransaction { Expense = expense, Transaction = second });

        var entity = db.Model.FindEntityType(typeof(ExpenseTransaction))!;
        Assert.Equal("Finance", entity.GetSchema());
        Assert.Equal("ExpenseTransaction", entity.GetTableName());
        Assert.Equal(new[] { "ExpenseId", "TransactionId" }, entity.GetProperties().Select(x => x.Name));
        Assert.Equal(new[] { "ExpenseId", "TransactionId" },
            entity.FindPrimaryKey()!.Properties.Select(x => x.Name));

        var view = new Model.View.Expense.Expense(expense);
        Assert.Equal(new[] { first.ValueId, second.ValueId }, view.Transactions.Select(x => x.Id));
        Assert.Empty(new Model.View.Expense.Expense(new Expense()).Transactions);
    }

    [Fact]
    public void PagedExpenseProjectionLoadsTransactionsAndFiltersByAccountOwner()
    {
        using var db = new MainContext("Server=localhost;Database=ModelOnly;Integrated Security=true");

        var sql = db.Expenses.Where(x => x.UserId == 2)
                    .IncludeAll()
                    .IncludeTransactions(2)
                    .OrderBy(x => x.Id)
                    .Select(x => new Model.View.Expense.Expense(x))
                    .Take(10)
                    .ToQueryString();

        Assert.Contains("[Finance].[ExpenseTransaction]", sql);
        Assert.Contains("[Finance].[Transaction]", sql);
        Assert.Contains("[Finance].[Account]", sql);
        Assert.Contains("[ValueId]", sql);
    }
}

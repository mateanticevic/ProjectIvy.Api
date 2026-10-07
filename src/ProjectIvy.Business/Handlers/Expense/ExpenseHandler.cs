using System.Collections.Concurrent;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;
using Dapper;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Logging;
using ProjectIvy.Business.Caching;
using ProjectIvy.Business.Exceptions;
using ProjectIvy.Business.Handlers.File;
using ProjectIvy.Business.MapExtensions;
using ProjectIvy.Common.Extensions;
using ProjectIvy.Data.Databases.Main.Queries;
using ProjectIvy.Data.DbContexts;
using ProjectIvy.Data.Extensions.Entities;
using ProjectIvy.Data.Extensions;
using ProjectIvy.Data.Sql.Main.Scripts;
using ProjectIvy.Data.Sql;
using ProjectIvy.Model.Binding.Expense;
using ProjectIvy.Model.Binding.File;
using ProjectIvy.Model.Binding;
using ProjectIvy.Model.Constants.Database;
using ProjectIvy.Model.View.ExpenseType;
using ProjectIvy.Model.View;
using SixLabors.ImageSharp.PixelFormats;
using SixLabors.ImageSharp;
using ZXing.ImageSharp;
using iText.Kernel.Pdf.Canvas.Parser;
using iText.Kernel.Pdf;
using View = ProjectIvy.Model.View.Expense;

namespace ProjectIvy.Business.Handlers.Expense;

public class ExpenseHandler : Handler<ExpenseHandler>, IExpenseHandler
{
    private static readonly ConcurrentDictionary<int, SemaphoreSlim> _createLocks = new();

    private readonly IFileHandler _fileHandler;

    public ExpenseHandler(IHandlerContext<ExpenseHandler> context,
                          IFileHandler fileHandler,
                          IMemoryCache memoryCache) : base(context, memoryCache, nameof(ExpenseHandler))
    {
        _fileHandler = fileHandler;
    }

    public async Task AddFile(string expenseValueId, string fileValueId, ExpenseFileBinding binding)
    {
        using var context = GetMainContext();
        int fileId = (await context.Files.GetIdAsync(fileValueId)).Value;
        int expenseId = (await context.Expenses.WhereUser(UserId).GetIdAsync(expenseValueId)).Value;
        int expenseFileTypeId = (await context.ExpenseFileTypes.GetIdAsync(binding.TypeId)).Value;

        var entity = new Model.Database.Main.Finance.ExpenseFile()
        {
            ExpenseFileTypeId = expenseFileTypeId,
            ExpenseId = expenseId,
            FileId = fileId,
            Name = binding.Name
        };

        await context.ExpenseFiles.AddAsync(entity);
        await context.SaveChangesAsync();
        ClearCache();
    }

    public async Task<int> Count(ExpenseGetBinding binding)
    {
        using var context = GetMainContext();
        var result = context.Expenses.WhereUser(UserId)
                            .Where(binding, context);

        return await result.CountAsync();
    }

    public async Task<IEnumerable<KeyValuePair<string, int>>> CountByDay(ExpenseGetBinding binding)
    {
        using var context = GetMainContext();
        var to = binding.To ?? DateTime.Now;

        return (await context.Expenses.WhereUser(UserId)
                               .Where(binding, context)
                               .GroupBy(x => x.Date)
                               .OrderByDescending(x => x.Key)
                               .Select(x => new KeyValuePair<DateTime, int>(x.Key, x.Count()))
                               .ToListAsync())
                               .FillMissingDates(x => x.Key, x => new KeyValuePair<DateTime, int>(x, 0), binding.From, to)
                               .Select(x => new KeyValuePair<string, int>(x.Key.ToString("yyyy-MM-dd"), x.Value));
    }

    public async Task<IEnumerable<KeyValuePair<int, int>>> CountByDayOfWeek(ExpenseGetBinding binding)
    {
        using var context = GetMainContext();
        DateTime FirstSunday = new DateTime(2000, 1, 2);
        var to = binding.To ?? DateTime.Now;

        return await context.Expenses
            .WhereUser(UserId)
            .Where(binding, context)
            .GroupBy(x => ((int)EF.Functions.DateDiffDay((DateTime?)FirstSunday, (DateTime?)x.Date) - 1) % 7)
            .OrderBy(x => x.Key)
            .Select(x => new KeyValuePair<int, int>(x.Key, x.Count()))
            .ToListAsync();
    }

    public async Task<IEnumerable<KeyValuePair<int, int>>> CountByMonth(ExpenseGetBinding binding)
    {
        using var context = GetMainContext();
        var to = binding.To ?? DateTime.Now;

        return await context.Expenses.WhereUser(UserId)
                               .Where(binding, context)
                               .GroupBy(x => x.Date.Month)
                               .OrderBy(x => x.Key)
                               .Select(x => new KeyValuePair<int, int>(x.Key, x.Count()))
                               .ToListAsync();
    }

    public async Task<IEnumerable<KeyValuePair<string, int>>> CountByMonthOfYear(ExpenseGetBinding binding)
    {
        using var context = GetMainContext();
        var to = binding.To ?? DateTime.Now;

        return (await context.Expenses.WhereUser(UserId)
                               .Where(binding, context)
                               .GroupBy(x => new { x.Date.Year, x.Date.Month })
                               .Select(x => new GroupedByMonth<int>(x.Count(), x.Key.Year, x.Key.Month))
                               .ToListAsync())
                               .FillMissingMonths(datetime => new GroupedByMonth<int>(0, datetime.Year, datetime.Month), binding.From, to)
                               .Select(x => new KeyValuePair<string, int>($"{x.Year}-{x.Month}", x.Data));
    }

    public async Task<PagedView<KeyValuePair<ExpenseType, int>>> CountByType(ExpenseGetBinding binding)
    {
        using var context = GetMainContext();
        return await context.Expenses.WhereUser(UserId)
                               .Where(binding, context)
                               .Include(x => x.ExpenseType)
                               .GroupBy(x => new
                               {
                                   x.ExpenseType.Name,
                                   x.ExpenseType.ValueId
                               })
                               .OrderByDescending(x => x.Count())
                               .Select(x => new KeyValuePair<ExpenseType, int>(new ExpenseType()
                               {
                                   Id = x.Key.ValueId,
                                   Name = x.Key.Name,
                               }, x.Count()))
                               .ToPagedViewAsync(binding);
    }

    public async Task<PagedView<KeyValuePair<Model.View.Vendor.Vendor, int>>> CountByVendor(ExpenseGetBinding binding)
    {
        using var context = GetMainContext();
        return await context.Expenses.WhereUser(UserId)
                               .Where(binding, context)
                               .Include(x => x.Vendor)
                               .GroupBy(x => new
                               {
                                   x.Vendor.ValueId,
                                   x.Vendor.Name
                               })
                               .OrderByDescending(x => x.Count())
                               .Select(x => new KeyValuePair<Model.View.Vendor.Vendor, int>(new() { Id = x.Key.ValueId, Name = x.Key.Name }, x.Count()))
                               .ToPagedViewAsync(binding);
    }

    public async Task<IEnumerable<KeyValuePair<int, int>>> CountByYear(ExpenseGetBinding binding)
    {
        using var context = GetMainContext();
        var to = binding.To ?? DateTime.Now;

        return (await context.Expenses.WhereUser(UserId)
                               .Where(binding, context)
                               .GroupBy(x => x.Date.Year)
                               .Select(x => new KeyValuePair<int, int>(x.Key, x.Count()))
                               .ToListAsync())
                               .FillMissingYears(year => new KeyValuePair<int, int>(0, year), binding.From?.Year, to.Year);
    }

    public async Task<int> CountTypes(ExpenseGetBinding binding)
    {
        using var context = GetMainContext();
        return await context.Expenses.WhereUser(UserId)
                               .Include(x => x.Vendor)
                               .Where(binding, context)
                               .GroupBy(x => x.ExpenseTypeId)
                               .CountAsync();
    }

    public async Task<int> CountVendors(ExpenseGetBinding binding)
    {
        using var context = GetMainContext();
        return await context.Expenses.WhereUser(UserId)
                               .Include(x => x.Vendor)
                               .Where(binding, context)
                               .Where(x => x.VendorId.HasValue)
                               .GroupBy(x => x.VendorId)
                               .CountAsync();
    }

    public async Task<string> Create(ExpenseBinding binding)
    {
        if (!string.IsNullOrWhiteSpace(binding.VendorName))
            binding.VendorId = await CreateVendor(binding.VendorName);

        var userLock = _createLocks.GetOrAdd(UserId, _ => new SemaphoreSlim(1, 1));
        await userLock.WaitAsync();
        try
        {
            using var db = GetMainContext();
            var entity = await binding.ToEntity(db);
            entity.UserId = UserId;
            entity.ValueId = (await db.Expenses.NextValueIdAsync(UserId)).ToString();

            await db.Expenses.AddAsync(entity);
            ResolveTransaction(db, entity);

            await db.SaveChangesAsync();
            ClearCache();

            return entity.ValueId;
        }
        finally
        {
            userLock.Release();
        }
    }

    public async Task CreateFromFile(FileBinding binding)
    {
        var stringBuilder = new StringBuilder();
        var fileType = FileType.PDF;

        if (binding.MimeType != "application/pdf")
        {
            fileType = FileType.IMG;
            var image = Image.Load<Rgba32>(binding.Data);
            var reader = new BarcodeReader<Rgba32>();

            var result = reader.Decode(image);
            stringBuilder.Append(result.Text);
        }

        using (Stream stream = new MemoryStream(binding.Data))
        {
            using var pdfReader = new PdfReader(stream);
            using var pdfDocument = new PdfDocument(pdfReader);

            for (int i = 1; i <= pdfDocument.GetNumberOfPages(); i++)
            {
                var page = pdfDocument.GetPage(i);
                stringBuilder.Append(PdfTextExtractor.GetTextFromPage(page));
            }
        }

        using var context = GetMainContext();
        var templates = await context.ExpenseFileTemplates.WhereUser(UserId)
                                                          .Where(x => x.FileType == fileType.ToString())
                                                          .ToListAsync();

        var user = await context.Users.Where(x => x.Id == UserId).SingleAsync();

        string text = stringBuilder.ToString();

        var template = templates.FirstOrDefault(x => new Regex(x.MatchRegex).Match(text).Success);

        if (template is null)
            throw new ResourceNotFoundException();

        var yearRegexMatch = template.YearRegex is null ? null : new Regex(template.YearRegex).Match(text);
        var monthRegexMatch = template.MonthRegex is null ? null : new Regex(template.MonthRegex).Match(text);
        var dayRegexMatch = template.DayRegex is null ? null : new Regex(template.DayRegex).Match(text);

        int? year = yearRegexMatch?.Success == true ? int.Parse(yearRegexMatch.Groups[1].Value) : null;
        int? month = monthRegexMatch?.Success == true ? int.Parse(monthRegexMatch.Groups[1].Value) : null;
        int? day = dayRegexMatch?.Success == true ? int.Parse(dayRegexMatch.Groups[1].Value) : null;

        if (year.HasValue && year.Value.ToString().Length == 2)
            year += 2000;

        var amountRegex = new Regex(template.AmountRegex).Match(text);

        decimal amount = amountRegex.Groups.Count == 3 ? decimal.Parse($"{amountRegex.Groups[1].Value}.{amountRegex.Groups[2].Value}", CultureInfo.InvariantCulture) : decimal.Parse(amountRegex.Groups[1].Value);

        var expense = new Model.Database.Main.Finance.Expense()
        {
            Amount = amount,
            Comment = template.CommentTemplate,
            CurrencyId = template.CurrencyId ?? user.DefaultCurrencyId,
            Date = DateTime.Now,
            DatePaid = DateTime.Now,
            ExpenseTypeId = template.ExpenseTypeId,
            NeedsReview = true,
            PaymentTypeId = template.PaymentTypeId,
            UserId = UserId,
            ValueId = (await context.Expenses.NextValueIdAsync(UserId)).ToString(),
            VendorId = template.VendorId,
        };

        if (expense.Comment is not null)
            expense.Comment = expense.Comment.Replace("{year}", year?.ToString() ?? string.Empty)
                                             .Replace("{month}", month?.ToString("D2") ?? string.Empty)
                                             .Replace("{day}", day?.ToString() ?? string.Empty);

        if (year.HasValue && month.HasValue && (day.HasValue || template.DefaultDayOfMonth is not null))
        {
            int dayOfMonth = day ?? (template.DefaultDayOfMonth.Value > 0
                                        ? template.DefaultDayOfMonth.Value
                                        : DateTime.DaysInMonth(year.Value, month.Value) + template.DefaultDayOfMonth.Value + 1);
            expense.Date = new DateTime(year.Value, month.Value, dayOfMonth);
        }

        if (fileType == FileType.IMG)
            binding.ImageResize = 0.5f;

        var file = await _fileHandler.UploadFileInternal(binding);
        var expenseFile = new Model.Database.Main.Finance.ExpenseFile()
        {
            Expense = expense,
            FileId = file.Id,
            ExpenseFileTypeId = 1
        };

        await context.Expenses.AddAsync(expense);
        await context.ExpenseFiles.AddAsync(expenseFile);
        await context.SaveChangesAsync();
        ClearCache();
    }

    private async Task<string> CreateVendor(string name)
    {
        using var context = GetMainContext();
        var entity = new Model.Database.Main.Finance.Vendor()
        {
            Name = name,
            ValueId = name.Replace(" ", "-").ToLowerInvariant()
        };
        await context.Vendors.AddAsync(entity);
        await context.SaveChangesAsync();
        return entity.ValueId;
    }

    public async Task Delete(string valueId)
    {
        using var db = GetMainContext();
        var entity = await db.Expenses.WhereUser(UserId)
                                .SingleOrDefaultAsync(x => x.ValueId == valueId);

        db.Expenses.Remove(entity);
        await db.SaveChangesAsync();
        ClearCache();
    }

    public async Task<View.Expense> Get(string expenseId)
    {
        using var context = GetMainContext();
        var expense = await context.Expenses.Include(x => x.ExpenseType)
                                      .Include(x => x.Currency)
                                      .Include(x => x.Poi)
                                      .Include(x => x.Vendor)
                                      .WhereUser(UserId)
                                      .SingleOrDefaultAsync(x => x.ValueId == expenseId);

        if (expense == null)
            throw new ResourceNotFoundException();

        return new View.Expense(expense);
    }

    public async Task<PagedView<View.Expense>> Get(ExpenseGetBinding binding)
    {
        return await MemoryCache.GetOrCreateAsync(BuildUserCacheKey(CacheKeyGenerator.ExpensesGet(binding)),
            async cacheEntry =>
            {
                AddCacheKey(cacheEntry.Key.ToString());
                cacheEntry.AbsoluteExpirationRelativeToNow = TimeSpan.FromMinutes(1);
                return await GetNonCached(binding);
            });
    }

    public async Task<IEnumerable<View.ExpenseFile>> GetFiles(string expenseId)
    {
        using var context = GetMainContext();
        return (await context.Expenses.IncludeAll()
                               .WhereUser(UserId)
                               .SingleOrDefaultAsync(x => x.ValueId == expenseId))
                               .ExpenseFiles
                               .Select(x => new View.ExpenseFile(x))
                               .ToList();
    }

    public async Task<PagedView<View.Expense>> GetNonCached(ExpenseGetBinding binding)
    {
        try
        {
            using var context = GetMainContext();
            return await context.Expenses.WhereUser(UserId)
                                   .IncludeAll()
                                   .Where(binding, context)
                                   .OrderBy(binding)
                                   .ThenByDescending(x => x.Created)
                                   .Select(x => new View.Expense(x))
                                   .ToPagedViewAsync(binding);
        }
        catch (Exception e)
        {
            Logger.LogError(e, "Error in GetNonCached with binding {@binding}", binding);
            throw;
        }
    }

    public async Task<IEnumerable<string>> GetTopDescriptions(ExpenseGetBinding binding)
    {
        using var context = GetMainContext();
        return await context.Expenses
                            .WhereUser(UserId)
                            .Where(binding, context)
                            .Where(x => !string.IsNullOrEmpty(x.Comment))
                            .GroupBy(x => x.Comment)
                            .Select(x => new { x.Key, Count = x.Count() })
                            .OrderByDescending(x => x.Count)
                            .Take(5)
                            .Select(x => x.Key)
                            .ToListAsync();
    }

    private void ResolveTransaction(MainContext context, Model.Database.Main.Finance.Expense expense)
    {
        // if (expense.Transaction is not null)
        //     context.Transactions.Remove(expense.Transaction);

        // if (expense.PaymentTypeId == (int)Model.Constants.Database.PaymentType.Cash)
        // {
        //     int? accountId = context.Accounts.SingleOrDefault(x => !x.BankId.HasValue && x.CurrencyId == expense.CurrencyId)?.Id;

        //     if (accountId.HasValue)
        //     {
        //         var transaction = new Model.Database.Main.Finance.Transaction()
        //         {
        //             AccountId = accountId.Value,
        //             Amount = -expense.Amount,
        //             Created = expense.Date
        //         };
        //         expense.Transaction = transaction;
        //         context.Transactions.Add(transaction);
        //     }
        // }
    }

    public async Task<decimal> SumAmount(ExpenseSumGetBinding binding)
    {
        string cacheKey = BuildUserCacheKey(CacheKeyGenerator.ExpensesSumAmount(binding));
        var x = await MemoryCache.GetOrCreateAsync(cacheKey,
            async cacheEntry =>
            {
                AddCacheKey(cacheKey);
                cacheEntry.AbsoluteExpirationRelativeToNow = TimeSpan.FromMinutes(1);
                return await SumAmountNonCached(binding);
            });
        return x;
    }

    private async Task<decimal> SumAmount(GetExpenseSumQuery query)
    {
        if (query is null)
            return 0;

        using var sql = GetSqlConnection();
        return Math.Round(await sql.ExecuteScalarAsync<decimal>(SqlLoader.Load(SqlScripts.GetExpenseSumInDefaultCurrency), query), 2);
    }

    private async Task<decimal> SumAmount(ExpenseSumGetBinding binding, bool excludeFromMonthlySums)
        => excludeFromMonthlySums
            ? await SumAmountNonCached(binding, excludeFromMonthlySums)
            : await SumAmount(binding);

    public async Task<IEnumerable<KeyValuePair<DateTime, decimal>>> SumAmountByDay(ExpenseSumGetBinding binding)
        => await SumAmountByDay(await SumBindingToQuery(binding, excludeFromMonthlySums: true));

    private async Task<IEnumerable<KeyValuePair<DateTime, decimal>>> SumAmountByDay(GetExpenseSumQuery query)
    {
        using var sql = GetSqlConnection();
        return (await sql.QueryAsync<(DateTime, decimal)>(SqlLoader.Load(SqlScripts.GetExpenseSumByDay), query))
            .Select(x => new KeyValuePair<DateTime, decimal>(x.Item1, Math.Round(x.Item2, 2)))
            .ToList();
    }

    public async Task<IEnumerable<KeyValuePair<int, decimal>>> SumAmountByDayOfWeek(ExpenseSumGetBinding binding)
    {
        using var context = GetMainContext();
        var tasks = Enumerable.Range(0, 7)
                              .Select(x => (DayOfWeek)x)
                              .Where(x => binding.Day == null || binding.Day.Contains(x))
                              .ToList()
                              .Select(x => new KeyValuePair<DayOfWeek, Task<decimal>>(
                                  x,
                                  SumAmount(binding.OverrideDay(x), excludeFromMonthlySums: true))
                              )
                              .ToList();

        await Task.WhenAll(tasks.Select(x => x.Value));
        return tasks.Select(x => new KeyValuePair<int, decimal>(x.Key == DayOfWeek.Sunday ? 6 : (int)x.Key - 1, x.Value.Result))
                    .OrderBy(x => x.Key);
    }

    public async Task<IEnumerable<KeyValuePair<int, decimal>>> SumAmountByMonth(ExpenseSumGetBinding binding)
    {
        using var context = GetMainContext();
        var tasks = Enumerable.Range(1, 12)
                              .Select(x => new KeyValuePair<int, Task<decimal>>(
                                  x,
                                  SumAmount(binding.OverrideMonth(x), excludeFromMonthlySums: true)));

        await Task.WhenAll(tasks.Select(x => x.Value));
        return tasks.Select(x => new KeyValuePair<int, decimal>(x.Key, x.Value.Result));
    }

    public async Task<IEnumerable<KeyValuePair<string, decimal>>> SumAmountByMonthOfYear(ExpenseSumGetBinding binding)
    {
        using var context = GetMainContext();
        var from = binding.From ?? (await context.Expenses.WhereUser(UserId).OrderBy(x => x.Date).FirstOrDefaultAsync()).Date;
        var to = binding.To ?? DateTime.Now;

        var periods = from.RangeMonthsClosed(to)
                          .Select(x => new FilteredBinding(x.from, x.to))
                          .ToList();

        var tasks = periods.Select(x => new KeyValuePair<FilteredBinding, Task<decimal>>(x, SumAmount(binding.OverrideFromTo<ExpenseSumGetBinding>(x.From, x.To), excludeFromMonthlySums: true))).ToList();

        await Task.WhenAll(tasks.Select(x => x.Value));

        return tasks.Select(x => new KeyValuePair<string, decimal>($"{x.Key.From.Value.Year}-{x.Key.From.Value.Month}-1", x.Value.Result));
    }

    public async Task<IEnumerable<KeyValuePair<int, decimal>>> SumAmountByYear(ExpenseSumGetBinding binding)
    {
        using var context = GetMainContext();
        int startYear = (await context.Expenses.WhereUser(UserId)
                                        .Where(binding, context)
                                        .OrderBy(x => x.Date)
                                        .FirstOrDefaultAsync()).Date.Year;
        int endYear = binding.To?.Year ?? DateTime.Now.Year;

        var years = Enumerable.Range(startYear, endYear - startYear + 1);

        var periods = years.Select(x => new FilteredBinding(new DateTime(x, 1, 1), new DateTime(x, 12, 31)));

        var tasks = periods.Select(x => new KeyValuePair<int, Task<decimal>>(x.From.Value.Year, SumAmount(binding.OverrideFromTo<ExpenseSumGetBinding>(x.From, x.To)))).ToList();

        await Task.WhenAll(tasks.Select(x => x.Value));

        return tasks.Select(x => new KeyValuePair<int, decimal>(x.Key, x.Value.Result));
    }

    private async Task<decimal> SumAmountNonCached(ExpenseSumGetBinding binding)
        => await SumAmountNonCached(binding, excludeFromMonthlySums: false);

    private async Task<decimal> SumAmountNonCached(ExpenseSumGetBinding binding, bool excludeFromMonthlySums)
        => await SumAmount(await SumBindingToQuery(binding, excludeFromMonthlySums));

    private async Task<GetExpenseSumQuery> SumBindingToQuery(ExpenseSumGetBinding binding, bool excludeFromMonthlySums = false)
    {
        using var context = GetMainContext();
        int targetCurrencyId = await context.GetCurrencyIdAsync(binding.TargetCurrencyId, UserId);

        var expenseIds = await context.Expenses.WhereUser(UserId)
                                               .Where(binding, context)
                                               .Select(x => x.Id)
                                               .ToListAsync();

        if (!expenseIds.Any())
            return null;

        return new GetExpenseSumQuery()
        {
            ExpenseIds = expenseIds,
            TargetCurrencyId = targetCurrencyId,
            UserId = UserId,
            ExcludeFromMonthlySums = excludeFromMonthlySums
        };
    }

    public async Task<IEnumerable<KeyValuePair<Model.View.Currency.Currency, decimal>>> SumByCurrency(ExpenseSumGetBinding binding)
    {
        using var context = GetMainContext();
        return await context.Expenses.WhereUser(UserId)
                                     .Where(binding, context)
                                     .Include(x => x.Currency)
                                     .GroupBy(x => new { x.Currency.ValueId, x.Currency.Name })
                                     .Select(x => new KeyValuePair<Model.View.Currency.Currency, decimal>(
                                         new()
                                         {
                                             Id = x.Key.ValueId,
                                             Name = x.Key.Name
                                         }, x.Sum(y => y.Amount)
                                         ))
                                     .ToListAsync();
    }

    public async Task<IEnumerable<KeyValuePair<string, IEnumerable<KeyValuePair<string, decimal>>>>> SumByMonthOfYearByType(ExpenseSumGetBinding binding)
    {
        using var context = GetMainContext();
        var from = binding.From ?? (await context.Expenses.WhereUser(UserId).OrderBy(x => x.Date).FirstOrDefaultAsync()).Date;
        var to = binding.To ?? DateTime.Now;

        var periods = from.RangeMonthsClosed(to)
                          .Select(x => new FilteredBinding(x.from, x.to))
                          .ToList();

        var tasks = periods.Select(x => new KeyValuePair<FilteredBinding, Task<IEnumerable<KeyValuePair<string, decimal>>>>(x, SumByType(binding.OverrideFromTo<ExpenseSumGetBinding>(x.From, x.To), excludeFromMonthlySums: true)));
        await Task.WhenAll(tasks.Select(x => x.Value));

        return tasks.Select(x => new KeyValuePair<string, IEnumerable<KeyValuePair<string, decimal>>>($"{x.Key.From.Value.Year}-{x.Key.From.Value.Month}-1", x.Value.Result));
    }

    public async Task<IEnumerable<KeyValuePair<string, decimal>>> SumByType(ExpenseSumGetBinding binding)
        => await SumByType(binding, excludeFromMonthlySums: false);

    private async Task<IEnumerable<KeyValuePair<string, decimal>>> SumByType(ExpenseSumGetBinding binding, bool excludeFromMonthlySums)
    {
        using var sql = GetSqlConnection();
        var results = await sql.QueryAsync<(int TypeId, string TypeValueId, decimal Amount)>(SqlLoader.Load(SqlScripts.GetExpenseSumByType), await SumBindingToQuery(binding, excludeFromMonthlySums));

        using var context = GetMainContext();

        if (binding.ByBaseType)
        {
            var typeIds = results.Select(x => x.TypeId).ToList();
            var types = context.ExpenseTypes.GetAll()
                                            .Where(x => typeIds.Contains(x.Id))
                                            .Select(x => (x.Id, x.ToParentType().ValueId))
                                            .Distinct()
                                            .ToList();

            return results.Join(types, x => x.TypeId, x => x.Id, (a, b) => (b.ValueId, a.Amount))
                          .GroupBy(x => x.ValueId)
                          .Select(x => new KeyValuePair<string, decimal>(x.Key, Math.Round(x.Sum(y => y.Amount), 2)));
        }
        else
            return results.Select(x => new KeyValuePair<string, decimal>(x.TypeValueId, Math.Round(x.Amount, 2))).ToList();
    }

    public async Task<IEnumerable<KeyValuePair<short, IEnumerable<KeyValuePair<string, decimal>>>>> SumByYearByType(ExpenseSumGetBinding binding)
    {
        using var context = GetMainContext();
        int startYear = (await context.Expenses.WhereUser(UserId)
                            .Where(binding, context)
                            .OrderBy(x => x.Date)
                            .FirstOrDefaultAsync()).Date.Year;
        int endYear = binding.To?.Year ?? DateTime.Now.Year;

        var years = Enumerable.Range(startYear, endYear - startYear + 1);

        var periods = years.Select(x => new FilteredBinding(new DateTime(x, 1, 1), new DateTime(x, 12, 31)));

        var tasks = periods.Select(x => new KeyValuePair<short, Task<IEnumerable<KeyValuePair<string, decimal>>>>((short)x.From.Value.Year, SumByType(binding.OverrideFromTo<ExpenseSumGetBinding>(x.From, x.To))));
        await Task.WhenAll(tasks.Select(x => x.Value));

        return tasks.Select(x => new KeyValuePair<short, IEnumerable<KeyValuePair<string, decimal>>>(x.Key, x.Value.Result));
    }

    public async Task<IEnumerable<string>> Split(string valueId, ExpenseSplitBinding binding)
    {
        if (binding?.Amount is null || binding.Expenses is null || binding.Expenses.Count == 0
            || binding.Expenses.Any(x => x?.Amount is null))
            throw new InvalidRequestException("Provide an amount for the original expense and at least one additional expense.");

        var parts = new[] { (ExpenseSplitPartBinding)binding }.Concat(binding.Expenses).ToList();
        if (parts.Any(x => decimal.Round(x.Amount.Value, 2) != x.Amount.Value))
            throw new InvalidRequestException("Expense amounts must have at most two decimal places.");

        var userLock = _createLocks.GetOrAdd(UserId, _ => new SemaphoreSlim(1, 1));
        await userLock.WaitAsync();
        try
        {
            using var context = GetMainContext();
            using var transaction = await context.Database.BeginTransactionAsync(System.Data.IsolationLevel.Serializable);
            var original = await context.Expenses.WhereUser(UserId)
                .Include(x => x.ExpenseFiles)
                .SingleOrDefaultAsync(x => x.ValueId == valueId);
            if (original is null)
                throw new ResourceNotFoundException();

            if (parts.Sum(x => x.Amount.Value) != original.Amount)
                throw new InvalidRequestException("Split amounts must add up to the original expense amount.");
            if (original.ParentAmount.HasValue && original.Amount == 0)
                throw new InvalidRequestException("Cannot allocate a parent-currency amount for a zero-total expense.");

            var typeIds = new List<int>();
            foreach (var part in parts)
            {
                if (part.ExpenseTypeId is null)
                    typeIds.Add(original.ExpenseTypeId);
                else
                {
                    var typeId = await context.ExpenseTypes.GetIdAsync(part.ExpenseTypeId);
                    if (!typeId.HasValue)
                        throw new InvalidRequestException($"Unknown expense type: {part.ExpenseTypeId}");
                    typeIds.Add(typeId.Value);
                }
            }

            var includedTrips = await context.TripExpensesIncluded.Where(x => x.ExpenseId == original.Id).ToListAsync();
            var excludedTrips = await context.TripExpensesExcluded.Where(x => x.ExpenseId == original.Id).ToListAsync();
            int nextValueId = await context.Expenses.NextValueIdAsync(UserId);
            var ids = new List<string> { original.ValueId };
            decimal allocatedParentAmount = 0;
            for (int i = 1; i < parts.Count; i++)
            {
                // Clone mapped scalar values, retaining foreign keys without copying the identity or navigations.
                var expense = (Model.Database.Main.Finance.Expense)context.Entry(original).CurrentValues.ToObject();
                expense.Id = 0;
                expense.ValueId = (nextValueId++).ToString(CultureInfo.InvariantCulture);
                expense.Created = DateTime.Now;
                expense.Modified = expense.Created;
                expense.Amount = parts[i].Amount.Value;
                expense.ExpenseTypeId = typeIds[i];
                expense.Comment = parts[i].Comment ?? original.Comment;
                if (original.ParentAmount.HasValue)
                {
                    expense.ParentAmount = decimal.Round(original.ParentAmount.Value * expense.Amount / original.Amount, 2);
                    allocatedParentAmount += expense.ParentAmount.Value;
                }
                context.Expenses.Add(expense);
                foreach (var file in original.ExpenseFiles)
                    context.ExpenseFiles.Add(new Model.Database.Main.Finance.ExpenseFile
                    {
                        Expense = expense, FileId = file.FileId,
                        ExpenseFileTypeId = file.ExpenseFileTypeId, Name = file.Name
                    });
                foreach (var trip in includedTrips)
                    context.TripExpensesIncluded.Add(new Model.Database.Main.Travel.TripExpenseInclude
                    {
                        Expense = expense, TripId = trip.TripId
                    });
                foreach (var trip in excludedTrips)
                    context.TripExpensesExcluded.Add(new Model.Database.Main.Travel.TripExpenseExclude
                    {
                        Expense = expense, TripId = trip.TripId
                    });
                ids.Add(expense.ValueId);
            }

            original.Amount = binding.Amount.Value;
            original.ExpenseTypeId = typeIds[0];
            original.Comment = binding.Comment ?? original.Comment;
            original.Modified = DateTime.Now;
            if (original.ParentAmount.HasValue)
                original.ParentAmount -= allocatedParentAmount;

            await context.SaveChangesAsync();
            await transaction.CommitAsync();
            ClearCache();
            return ids;
        }
        finally
        {
            userLock.Release();
        }
    }

    public async Task<bool> Update(ExpenseBinding binding)
    {
        if (!string.IsNullOrWhiteSpace(binding.VendorName))
            binding.VendorId = await CreateVendor(binding.VendorName);

        using var context = GetMainContext();
        var entity = await context.Expenses.WhereUser(UserId)
                                     .SingleOrDefaultAsync(x => x.ValueId == binding.Id);

        entity = await binding.ToEntity(context, entity);

        context.Expenses.Update(entity);
        ResolveTransaction(context, entity);

        await context.SaveChangesAsync();
        ClearCache();

        return true;
    }
}

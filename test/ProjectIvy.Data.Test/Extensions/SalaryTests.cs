using ProjectIvy.Data.Extensions;
using ProjectIvy.Data.Extensions.Entities;
using ProjectIvy.Model.Binding.Salary;
using ProjectIvy.Model.Database.Main.Common;
using ProjectIvy.Model.Database.Main.Finance;

namespace ProjectIvy.Data.Test.Extensions;

public class SalaryTests
{
    [Fact]
    public void CompanyFilterMatchesPublicIdAndCanBeOmitted()
    {
        var salaries = new[]
        {
            new Salary { Id = 1, CompanyId = 10, Company = new Company { Id = 10, ValueId = "acme" } },
            new Salary { Id = 2, CompanyId = 20, Company = new Company { Id = 20, ValueId = "other" } },
            new Salary { Id = 3 }
        }.AsQueryable();

        Assert.Equal(new[] { 1 }, salaries.Where(new SalaryGetBinding { CompanyId = "acme" }).Select(x => x.Id));
        Assert.Empty(salaries.Where(new SalaryGetBinding { CompanyId = "unknown" }));
        Assert.Equal(3, salaries.Where(new SalaryGetBinding()).Count());
        Assert.Equal(3, salaries.Where(new SalaryGetBinding { CompanyId = " " }).Count());
    }

    [Fact]
    public void FiltersIncludeDateBoundariesAndExcludeOtherOwnersAndCurrencies()
    {
        var from = new DateTime(2026, 1, 1);
        var to = from.AddMonths(1);
        var euro = new Currency { ValueId = "eur" };
        var salaries = new[]
        {
            new Salary { Id = 1, UserId = 1, Date = from.AddSeconds(-1), Currency = euro },
            new Salary { Id = 2, UserId = 1, Date = from, Currency = euro },
            new Salary { Id = 3, UserId = 1, Date = to, Currency = euro },
            new Salary { Id = 4, UserId = 1, Date = to.AddSeconds(1), Currency = euro },
            new Salary { Id = 5, UserId = 2, Date = from, Currency = euro },
            new Salary { Id = 6, UserId = 1, Date = from, Currency = new Currency { ValueId = "usd" } }
        };
        var query = salaries.AsQueryable().WhereUser(1);

        var result = query.Where(new SalaryGetBinding { From = from, To = to, CurrencyId = "eur" });

        Assert.Equal(new[] { 2, 3 }, result.Select(x => x.Id));
        Assert.Empty(query.Where(new SalaryGetBinding { CurrencyId = "unknown" }));
        Assert.Equal(5, query.Where(new SalaryGetBinding()).Count());
    }
}

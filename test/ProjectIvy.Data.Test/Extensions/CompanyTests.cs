using ProjectIvy.Data.Extensions;
using ProjectIvy.Data.Extensions.Entities;
using ProjectIvy.Model.Binding.Company;
using ProjectIvy.Model.Database.Main.Common;

namespace ProjectIvy.Data.Test.Extensions;

public class CompanyTests
{
    private static IQueryable<Company> Companies => new[]
    {
        new Company { Id = 1, ValueId = "acme", Name = "Acme Industries" },
        new Company { Id = 2, ValueId = "other", Name = "Other Industries" },
        new Company { Id = 3, ValueId = "supplier-three", Name = "Third Company" }
    }.AsQueryable();

    [Theory]
    [InlineData("ACME", 1)]
    [InlineData(" indus ", 2)]
    [InlineData("THIRD", 1)]
    [InlineData("SUPPLIER", 1)]
    [InlineData("missing", 0)]
    [InlineData(null, 3)]
    [InlineData("", 3)]
    [InlineData(" ", 3)]
    public void SearchMatchesNameOrPublicIdAndIgnoresBlankSearch(string? search, int count)
    {
        Assert.Equal(count, Companies.Where(new CompanyGetBinding { Search = search! }).Count());
    }

    [Fact]
    public void PagingReturnsFilteredTotalAndRequestedPage()
    {
        var binding = new CompanyGetBinding { Search = "industries", Page = 2, PageSize = 1 };
        var query = Companies.Where(binding).OrderBy(x => x.Name).ThenBy(x => x.ValueId);

        var result = query.ToPagedView(binding);

        Assert.Equal(2, result.Count);
        Assert.Equal("other", Assert.Single(result.Items).ValueId);

        binding.PageAll = true;
        Assert.Equal(2, query.ToPagedView(binding).Items.Count());
    }
}

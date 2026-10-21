using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging.Abstractions;
using Moq;
using ProjectIvy.Api.Controllers.Salary;
using ProjectIvy.Business.Handlers.Salary;
using ProjectIvy.Business.MapExtensions;
using ProjectIvy.Model.Binding.Salary;
using Xunit;
using Entity = ProjectIvy.Model.Database.Main.Finance.Salary;
using View = ProjectIvy.Model.View.Salary.Salary;

namespace ProjectIvy.Api.EndToEnd.Test;

public class SalaryContractTests
{
    [Theory]
    [InlineData("", "2026-10-21-1")]
    [InlineData("2026-10-21-1,2026-10-21-2", "2026-10-21-3")]
    [InlineData("2026-10-21-1,2026-10-21-3", "2026-10-21-4")]
    [InlineData("2026-10-21-9,2026-10-21-10", "2026-10-21-11")]
    [InlineData("2026-10-20-99,2025-10-21-5", "2026-10-21-1")]
    public void ValueIdUsesSalaryDateAndNextAvailablePaymentNumber(string existing, string expected)
    {
        Assert.Equal(expected, SalaryExtensions.GetNextValueId(new DateTime(2026, 10, 21, 15, 0, 0), existing.Split(',')));
    }

    [Fact]
    public async Task CreateReturnsResourceAndLocationWithGeneratedId()
    {
        var binding = new SalaryBinding { CurrencyId = "eur" };
        var view = new View(new Entity { Id = 42, ValueId = "2026-10-21-1" });
        var handler = new Mock<ISalaryHandler>(MockBehavior.Strict);
        handler.Setup(x => x.Create(binding)).ReturnsAsync(view);
        var controller = new SalaryController(NullLogger<SalaryController>.Instance, handler.Object);

        var result = await controller.Post(binding);

        var created = Assert.IsType<CreatedAtActionResult>(result.Result);
        Assert.Equal(nameof(SalaryController.Get), created.ActionName);
        Assert.Equal("2026-10-21-1", created.RouteValues!["id"]);
        Assert.Same(view, created.Value);
        handler.Verify(x => x.Create(binding), Times.Once);
    }

    [Fact]
    public async Task InvalidBodyDoesNotCreateOrUpdateSalary()
    {
        var handler = new Mock<ISalaryHandler>(MockBehavior.Strict);
        var controller = new SalaryController(NullLogger<SalaryController>.Instance, handler.Object);
        controller.ModelState.AddModelError("CurrencyId", "Required");
        var binding = new SalaryBinding();

        Assert.IsType<BadRequestObjectResult>((await controller.Post(binding)).Result);
        Assert.IsType<BadRequestObjectResult>(await controller.Put("2026-10-21-1", binding));
        handler.VerifyNoOtherCalls();
    }

    [Theory]
    [InlineData(null, 0, false)]
    [InlineData(" ", 0, false)]
    [InlineData("eur", 1000, true)]
    [InlineData("eur", 1001, false)]
    public void BindingValidatesCurrencyAndNoteLength(string? currencyId, int noteLength, bool expected)
    {
        var binding = new SalaryBinding { CurrencyId = currencyId!, Note = new string('x', noteLength) };

        Assert.Equal(expected, Validator.TryValidateObject(binding, new ValidationContext(binding), new List<ValidationResult>(), true));
    }
}

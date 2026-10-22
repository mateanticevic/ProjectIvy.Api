using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging.Abstractions;
using Moq;
using ProjectIvy.Api.Controllers.Income;
using ProjectIvy.Business.Handlers.Income;
using ProjectIvy.Model.Binding.Income;
using Xunit;
using Entity = ProjectIvy.Model.Database.Main.Finance.Income;
using View = ProjectIvy.Model.View.Income.Income;

namespace ProjectIvy.Api.EndToEnd.Test;

public class IncomeContractTests
{
    [Fact]
    public async Task PutUpdatesRequestedIncomeAndReturnsNoContent()
    {
        var binding = new IncomeBinding { Amount = 125, CurrencyId = "eur" };
        var handler = new Mock<IIncomeHandler>(MockBehavior.Strict);
        handler.Setup(x => x.Update("2026-10-22-1", binding)).Returns(Task.CompletedTask);
        var controller = new IncomeController(NullLogger<IncomeController>.Instance, handler.Object);

        var result = await controller.Put("2026-10-22-1", binding);

        Assert.IsType<NoContentResult>(result);
        handler.Verify(x => x.Update("2026-10-22-1", binding), Times.Once);
    }

    [Fact]
    public async Task InvalidBodyDoesNotUpdateIncome()
    {
        var handler = new Mock<IIncomeHandler>(MockBehavior.Strict);
        var controller = new IncomeController(NullLogger<IncomeController>.Instance, handler.Object);
        controller.ModelState.AddModelError("Amount", "Invalid amount");

        var result = await controller.Put("2026-10-22-1", new IncomeBinding());

        Assert.IsType<BadRequestObjectResult>(result);
        handler.VerifyNoOtherCalls();
    }

    [Fact]
    public void ResponseExposesPublicIncomeId()
    {
        var view = new View(new Entity { Id = 42, ValueId = "2026-10-22-1" });

        Assert.Equal("2026-10-22-1", view.Id);
    }
}

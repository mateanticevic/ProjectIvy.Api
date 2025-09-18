using System.ComponentModel;
using ModelContextProtocol.Server;
using ProjectIvy.Business.Handlers.Consumation;

namespace ProjectIvy.Api.Mcp;

[McpServerToolType]
public class BeerTools
{
    private readonly IConsumationHandler _consumationHandler;

    public BeerTools(IConsumationHandler consumationHandler)
    {
        _consumationHandler = consumationHandler;
    }

    [McpServerTool, Description("Total amount of beer drank in liters")]
    public decimal Sum(DateTime? from, DateTime? to)
    {
        return _consumationHandler.SumVolume(new Model.Binding.Consumation.ConsumationGetBinding()
        {
            From = from,
            To = to ?? DateTime.UtcNow,
        });
    }
}
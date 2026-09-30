using System.ComponentModel;
using System.Threading.Tasks;
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
    public async Task<decimal> Sum(DateTime? from, DateTime? to)
    {
        return await _consumationHandler.SumVolume(new Model.Binding.Consumation.ConsumationGetBinding()
        {
            From = from,
            To = to ?? DateTime.UtcNow,
        });
    }
}

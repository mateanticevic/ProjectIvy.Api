using Microsoft.AspNetCore.Authorization;
using ProjectIvy.Api.Constants;
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

    [Authorize(ApiScopes.BeerUser)]
    [McpServerTool(Name = "sum_beer"), Description("Returns the total volume of beer consumed in milliliters (mL).")]
    public async Task<decimal> Sum(DateTime? from, DateTime? to)
    {
        return await _consumationHandler.SumVolume(new Model.Binding.Consumation.ConsumationGetBinding()
        {
            From = from,
            To = to ?? DateTime.UtcNow,
        });
    }
}

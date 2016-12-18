using Microsoft.Extensions.Logging;

namespace AnticevicApi.BL.Handlers
{
    public interface IHandler
    {
        ILogger Logger { get; }
    }
}

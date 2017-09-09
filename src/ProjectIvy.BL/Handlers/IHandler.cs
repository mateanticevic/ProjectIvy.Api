using Microsoft.Extensions.Logging;

namespace ProjectIvy.BL.Handlers
{
    public interface IHandler
    {
        ILogger Logger { get; }
    }
}

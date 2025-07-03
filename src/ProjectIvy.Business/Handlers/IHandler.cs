using Microsoft.Extensions.Logging;

namespace ProjectIvy.Business.Handlers;

public interface IHandler
{
    ILogger Logger { get; }
}

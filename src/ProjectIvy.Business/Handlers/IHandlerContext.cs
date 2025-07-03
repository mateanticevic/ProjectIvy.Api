using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;

namespace ProjectIvy.Business.Handlers;

public interface IHandlerContext<THandler>
{
    IHttpContextAccessor Context { get; }

    ILogger<THandler> Logger { get; }
}

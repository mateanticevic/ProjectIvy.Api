using AnticevicApi.Common.Configuration;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace AnticevicApi.BL.Handlers
{
    public interface IHandlerContext<THandler>
    {
        IHttpContextAccessor Context { get; }

        ILogger<THandler> Logger { get; }

        IOptions<AppSettings> Settings { get; }
    }
}

using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using ProjectIvy.Common.Configuration;

namespace ProjectIvy.Business.Handlers
{
    public interface IHandlerContext<THandler>
    {
        IHttpContextAccessor Context { get; }

        ILogger<THandler> Logger { get; }

        IOptions<AppSettings> Settings { get; }
    }
}

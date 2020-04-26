using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;

namespace ProjectIvy.Business.Handlers
{
    public class HandlerContext<THandler> : IHandlerContext<THandler>
    {
        public HandlerContext(IHttpContextAccessor context, ILogger<THandler> logger)
        {
            Context = context;
            Logger = logger;
        }

        public IHttpContextAccessor Context { get; private set; }

        public ILogger<THandler> Logger { get; private set; }
    }
}

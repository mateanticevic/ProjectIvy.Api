﻿using AnticevicApi.Common.Configuration;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace AnticevicApi.BL.Handlers
{
    public class HandlerContext<THandler> : IHandlerContext<THandler>
    {
        public HandlerContext(IHttpContextAccessor context, ILogger<THandler> logger, IOptions<AppSettings> settings)
        {
            Context = context;
            Logger = logger;
            Settings = settings;
        }

        public IHttpContextAccessor Context { get; private set; }

        public ILogger<THandler> Logger { get; private set; }

        public IOptions<AppSettings> Settings { get; private set; }
    }
}
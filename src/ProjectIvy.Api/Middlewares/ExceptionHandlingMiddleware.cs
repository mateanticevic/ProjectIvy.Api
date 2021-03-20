using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using ProjectIvy.Business.Exceptions;
using System;
using System.Net;
using System.Threading.Tasks;

namespace ProjectIvy.Api.Middlewares
{
    public class ExceptionHandlingMiddleware
    {
        private readonly RequestDelegate _next;

        public ExceptionHandlingMiddleware(RequestDelegate next)
        {
            _next = next;
        }

        public static Task HandleException(HttpContext context, Exception e)
        {
            var statusCode = HttpStatusCode.InternalServerError;

            if (e is UnauthorizedException)
                statusCode = HttpStatusCode.Unauthorized;
            else if (e is InvalidRequestException)
                statusCode = HttpStatusCode.BadRequest;
            else if (e is ResourceExistsException)
                statusCode = HttpStatusCode.Conflict;
            else if (e is ResourceForbiddenException)
                statusCode = HttpStatusCode.Forbidden;
            else if (e is ResourceNotFoundException)
                statusCode = HttpStatusCode.NotFound;

            var result = JsonConvert.SerializeObject(new { error = e.Message });
            context.Response.ContentType = "application/json";
            context.Response.StatusCode = (int)statusCode;

            return context.Response.WriteAsync(result);
        }

        public async Task Invoke(HttpContext httpContext, ILogger<ExceptionHandlingMiddleware> logger)
        {
            try
            {
                await _next(httpContext);
            }
            catch (Exception e)
            {
                logger.LogError(e, string.Empty);
                await HandleException(httpContext, e);
            }
        }
    }
}

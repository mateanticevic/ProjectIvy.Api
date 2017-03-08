using AnticevicApi.BL.Exceptions;
using Microsoft.AspNetCore.Http;
using Newtonsoft.Json;
using System;
using System.Net;
using System.Threading.Tasks;

namespace AnticevicApi.Middleware
{
    public class ExceptionHandlingMiddleware
    {
        private readonly RequestDelegate _next;

        public ExceptionHandlingMiddleware(RequestDelegate next)
        {
            _next = next;
        }

        public async Task Invoke(HttpContext httpContext)
        {
            try
            {
                await _next(httpContext);
            }
            catch(Exception e)
            {
                await HandleException(httpContext, e);
            }
        }

        public static Task HandleException(HttpContext context, Exception e)
        {
            var statusCode = HttpStatusCode.InternalServerError;

            if (e is UnauthorizedAccessException)
                statusCode = HttpStatusCode.Unauthorized;
            else if (e is ResourceExistsException)
                statusCode = HttpStatusCode.Conflict;

            var result = JsonConvert.SerializeObject(new { error = e.Message });
            context.Response.ContentType = "application/json";
            context.Response.StatusCode = (int)statusCode;
            return context.Response.WriteAsync(result);
        }
    }
}

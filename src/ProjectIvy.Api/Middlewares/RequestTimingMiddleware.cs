using System.Diagnostics;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Builder;
using Serilog;

namespace ProjectIvy.Api.Middlewares;

public class RequestTimingMiddleware
{
    private readonly RequestDelegate _next;

    public RequestTimingMiddleware(RequestDelegate next)
    {
        _next = next;
    }

    public async Task InvokeAsync(HttpContext context)
    {
        var sw = Stopwatch.StartNew();
        try
        {
            await _next(context);
        }
        finally
        {
            sw.Stop();
            var scheme = context.Items.TryGetValue("ChosenAuthScheme", out var s) ? s : "(none)";
            Log.Information("REQ {Method} {Path} => {StatusCode} in {Elapsed} ms auth={AuthScheme}",
                context.Request.Method,
                context.Request.Path,
                context.Response?.StatusCode,
                sw.Elapsed.TotalMilliseconds.ToString("F1"),
                scheme);
        }
    }
}

public static class RequestTimingMiddlewareExtensions
{
    public static IApplicationBuilder UseRequestTiming(this IApplicationBuilder app)
    {
        return app.UseMiddleware<ProjectIvy.Api.Middlewares.RequestTimingMiddleware>();
    }
}
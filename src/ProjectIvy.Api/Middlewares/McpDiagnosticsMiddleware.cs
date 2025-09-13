using System.Diagnostics;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Serilog;
using Microsoft.AspNetCore.Builder;

namespace ProjectIvy.Api.Middlewares;

public class McpDiagnosticsMiddleware
{
    private readonly RequestDelegate _next;

    public McpDiagnosticsMiddleware(RequestDelegate next)
    {
        _next = next;
    }

    public async Task InvokeAsync(HttpContext context)
    {
        if (!context.Request.Path.StartsWithSegments("/mcp"))
        {
            await _next(context);
            return;
        }

        var sw = Stopwatch.StartNew();
        var traceId = context.TraceIdentifier;
        var scheme = context.Items.TryGetValue("ChosenAuthScheme", out var s) ? s : "(none)";
        Log.Information("MCP START {TraceId} {Method} {Path} auth={Auth} contentType={ContentType} accept={Accept}", traceId, context.Request.Method, context.Request.Path, scheme, context.Request.ContentType, context.Request.Headers["Accept"].ToString());
        try
        {
            await _next(context);
            sw.Stop();
            Log.Information("MCP END {TraceId} {StatusCode} elapsed={Elapsed}ms", traceId, context.Response?.StatusCode, sw.Elapsed.TotalMilliseconds.ToString("F1"));
        }
        catch (Exception ex)
        {
            sw.Stop();
            Log.Error(ex, "MCP ERROR {TraceId} elapsed={Elapsed}ms", traceId, sw.Elapsed.TotalMilliseconds.ToString("F1"));
            throw;
        }
    }
}

public static class McpDiagnosticsMiddlewareExtensions
{
    public static IApplicationBuilder UseMcpDiagnostics(this IApplicationBuilder app)
    {
        return app.UseMiddleware<ProjectIvy.Api.Middlewares.McpDiagnosticsMiddleware>();
    }
}
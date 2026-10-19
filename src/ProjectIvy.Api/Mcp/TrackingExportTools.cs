using System.ComponentModel;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using ModelContextProtocol.Protocol;
using ModelContextProtocol.Server;
using ProjectIvy.Api.Constants;
using ProjectIvy.Business.Handlers.Tracking;

namespace ProjectIvy.Api.Mcp;

[McpServerToolType]
public class TrackingExportTools(ITrackingExportHandler handler)
{
    [Authorize(ApiScopes.TrackingUser)]
    [McpServerTool(Name = "tracking_get", ReadOnly = true)]
    [Description("Returns a rendered PNG map with a chronological polyline of the authenticated user's trackings for a selected local day. Returns a message if no usable tracking points exist.")]
    public async Task<CallToolResult> Get(
        [Description("Required local calendar date in YYYY-MM-DD format.")] DateOnly date,
        [Description("Optional IANA timezone used for day boundaries; defaults to the authenticated user's DefaultTimezone.")] string timezone = null,
        CancellationToken cancellationToken = default)
    {
        try
        {
            var png = await handler.GetMap(date, timezone, cancellationToken);
            return new CallToolResult
            {
                Content = png is null
                    ? [new TextContentBlock { Text = $"No usable tracking points for {date:yyyy-MM-dd}{(string.IsNullOrWhiteSpace(timezone) ? "" : $" ({timezone})")}." }]
                    : [ImageContentBlock.FromBytes(png, "image/png")]
            };
        }
        catch (ArgumentException exception)
        {
            return Error(exception.Message);
        }
        catch (TimeoutException)
        {
            return Error("Tracking map rendering timed out. Please try again.");
        }
        catch (OperationCanceledException) { throw; }
        catch (Exception)
        {
            return Error("Unable to render the tracking map. Please try again.");
        }
    }

    private static CallToolResult Error(string message)
        => new() { IsError = true, Content = [new TextContentBlock { Text = message }] };
}

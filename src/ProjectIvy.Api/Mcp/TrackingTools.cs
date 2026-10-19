using System.ComponentModel;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using ModelContextProtocol.Protocol;
using ModelContextProtocol.Server;
using ProjectIvy.Api.Constants;
using ProjectIvy.Business.Handlers.Tracking;
using ProjectIvy.Model.View.Tracking;

namespace ProjectIvy.Api.Mcp;

[McpServerToolType]
public class TrackingTools
{
    private readonly ITrackingHandler _trackingHandler;
    private readonly ITrackingExportHandler _trackingExportHandler;

    public TrackingTools(ITrackingHandler trackingHandler, ITrackingExportHandler trackingExportHandler)
    {
        _trackingHandler = trackingHandler;
        _trackingExportHandler = trackingExportHandler;
    }

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
            var png = await _trackingExportHandler.GetMap(date, timezone, cancellationToken);
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

    [Authorize(ApiScopes.TrackingViewLast)]
    [McpServerTool(Name = "tracking_last_get", ReadOnly = true, UseStructuredContent = true)]
    [Description("Returns the authenticated user's last known location, including tracking coordinates, city, country, known location, and flight when available. Interpret available information in order of relevance, from most to least specific: flight, location, city, country, coordinates. Prefer the most specific available information when describing the user's location. Optionally returns the last location strictly before a UTC timestamp. Returns null if no matching tracking exists.")]
    public Task<TrackingLocation> GetLast(
        [Description("Optional UTC timestamp (ISO 8601, for example 2026-10-15T12:00:00Z). Returns the newest tracking strictly before this time; omit for the latest location. Flight selection also uses this time when provided.")] DateTime? at = null)
        => _trackingHandler.GetLastLocation(at);

    private static CallToolResult Error(string message)
        => new() { IsError = true, Content = [new TextContentBlock { Text = message }] };
}

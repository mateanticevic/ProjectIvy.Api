using System.ComponentModel;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using ModelContextProtocol.Server;
using ProjectIvy.Api.Constants;
using ProjectIvy.Business.Handlers.Tracking;
using ProjectIvy.Model.View.Tracking;

namespace ProjectIvy.Api.Mcp;

[McpServerToolType]
public class TrackingTools
{
    private readonly ITrackingHandler _trackingHandler;

    public TrackingTools(ITrackingHandler trackingHandler)
    {
        _trackingHandler = trackingHandler;
    }

    [Authorize(ApiScopes.TrackingViewLast)]
    [McpServerTool(Name = "tracking_last_get", ReadOnly = true, UseStructuredContent = true)]
    [Description("Returns the authenticated user's last known location, including tracking coordinates, city, country, known location, and flight when available. Interpret available information in order of relevance, from most to least specific: flight, location, city, country, coordinates. Prefer the most specific available information when describing the user's location. Optionally returns the last location strictly before a UTC timestamp. Returns null if no matching tracking exists.")]
    public Task<TrackingLocation> GetLast(
        [Description("Optional UTC timestamp (ISO 8601, for example 2026-10-15T12:00:00Z). Returns the newest tracking strictly before this time; omit for the latest location. Flight selection also uses this time when provided.")] DateTime? at = null)
        => _trackingHandler.GetLastLocation(at);
}

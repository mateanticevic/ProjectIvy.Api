using System.Globalization;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using ProjectIvy.Data.Extensions;
using SixLabors.Fonts;
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.Drawing;
using SixLabors.ImageSharp.Drawing.Processing;
using SixLabors.ImageSharp.PixelFormats;
using SixLabors.ImageSharp.Processing;
using TrackingEntity = ProjectIvy.Model.Database.Main.Tracking.Tracking;

namespace ProjectIvy.Business.Handlers.Tracking;

public class TrackingExportHandler : Handler<TrackingExportHandler>, ITrackingExportHandler
{
    internal const int Width = 1024;
    internal const int Height = 768;
    private const int Padding = 48;
    private static readonly Lazy<Font> LabelFont = new(() =>
    {
        using var stream = typeof(TrackingExportHandler).Assembly.GetManifestResourceStream(
            "ProjectIvy.Business.Assets.Fonts.Lato-Regular.ttf");
        return new FontCollection().Add(stream).CreateFont(16);
    });
    private readonly TrackingMapTileClient _tiles;

    public TrackingExportHandler(IHandlerContext<TrackingExportHandler> context, TrackingMapTileClient tiles)
        : base(context) => _tiles = tiles;

    public async Task<byte[]> GetMap(DateOnly date, string timezone = null, CancellationToken cancellationToken = default)
    {
        using var timeout = CancellationTokenSource.CreateLinkedTokenSource(cancellationToken);
        timeout.CancelAfter(TimeSpan.FromSeconds(30));
        try
        {
            using var db = GetMainContext();
            if (string.IsNullOrWhiteSpace(timezone))
            {
                timezone = await db.Users.Where(x => x.Id == UserId)
                    .Select(x => x.DefaultTimezone).SingleAsync(timeout.Token);
                if (string.IsNullOrWhiteSpace(timezone))
                    throw new ArgumentException("Your default timezone is not configured. Supply a timezone or set User.DefaultTimezone.");
            }
            var (from, to) = GetUtcBounds(date, timezone);
            var points = await SelectTrackings(db.Trackings.AsNoTracking(), UserId, from, to)
                .Select(x => new MapCoordinate((double)x.Latitude, (double)x.Longitude))
                .ToListAsync(timeout.Token);
            return await RenderMap(points, date, timezone, _tiles, timeout.Token);
        }
        catch (ArgumentException) { throw; }
        catch (OperationCanceledException) when (!cancellationToken.IsCancellationRequested)
        {
            Logger.LogWarning("Tracking map export timed out");
            throw new TimeoutException("Tracking map rendering timed out.");
        }
        catch (OperationCanceledException) { throw; }
        catch (Exception exception)
        {
            // HTTP exception messages can contain tile URLs. Do not log location data.
            Logger.LogError("Tracking map export failed ({ErrorType})", exception.GetType().Name);
            throw new InvalidOperationException("Unable to render the tracking map.");
        }
    }

    internal static (DateTime From, DateTime To) GetUtcBounds(DateOnly date, string timezone)
    {
        if (date == DateOnly.MaxValue)
            throw new ArgumentException("The date must allow a following day.", nameof(date));
        if (string.IsNullOrWhiteSpace(timezone) || !TimeZoneInfo.TryConvertIanaIdToWindowsId(timezone, out _))
            throw new ArgumentException("Timezone must be a valid IANA timezone identifier.", nameof(timezone));
        try
        {
            var zone = TimeZoneInfo.FindSystemTimeZoneById(timezone);
            return (GetUtcBoundary(date.ToDateTime(TimeOnly.MinValue), zone),
                GetUtcBoundary(date.AddDays(1).ToDateTime(TimeOnly.MinValue), zone));
        }
        catch (Exception exception) when (exception is TimeZoneNotFoundException or InvalidTimeZoneException or ArgumentException)
        {
            throw new ArgumentException("The selected day has invalid midnight boundaries in this timezone.", nameof(timezone));
        }
    }

    private static DateTime GetUtcBoundary(DateTime local, TimeZoneInfo zone)
    {
        // Some zones advance at midnight. Begin at the first existing local minute.
        while (zone.IsInvalidTime(local)) local = local.AddMinutes(1);
        // When midnight occurs twice, include both occurrences in the selected day.
        if (zone.IsAmbiguousTime(local))
            return DateTime.SpecifyKind(local - zone.GetAmbiguousTimeOffsets(local).Max(), DateTimeKind.Utc);
        return TimeZoneInfo.ConvertTimeToUtc(local, zone);
    }

    internal static IQueryable<TrackingEntity> SelectTrackings(IQueryable<TrackingEntity> query, int userId, DateTime from, DateTime to)
        => query.WhereUser(userId).WhereTimestampFromInclusive(from, to).OrderBy(x => x.Timestamp).ThenBy(x => x.Id);

    internal static async Task<byte[]> RenderMap(IEnumerable<MapCoordinate> coordinates, DateOnly date, string timezone,
        TrackingMapTileClient tiles, CancellationToken cancellationToken)
    {
        cancellationToken.ThrowIfCancellationRequested();
        var points = coordinates.Where(x => double.IsFinite(x.Latitude) && double.IsFinite(x.Longitude)
            && x.Latitude >= -90 && x.Latitude <= 90 && x.Longitude >= -180 && x.Longitude <= 180).ToArray();
        if (points.Length == 0) return null;

        var viewport = GetViewport(points);
        var size = 256 * (1 << viewport.Zoom);
        var tileCount = 1 << viewport.Zoom;
        var placements = new List<(int X, int Y, Task<byte[]> Data)>();
        // Each output pixel samples the same integer origin used for the route overlay.
        for (var y = (int)Math.Floor(viewport.Top / 256d); y <= (int)Math.Floor((viewport.Top + Height - 1) / 256d); y++)
        {
            if (y < 0 || y >= tileCount) continue;
            for (var x = (int)Math.Floor(viewport.Left / 256d); x <= (int)Math.Floor((viewport.Left + Width - 1) / 256d); x++)
            {
                var wrappedX = ((x % tileCount) + tileCount) % tileCount;
                placements.Add((x * 256 - viewport.Left, y * 256 - viewport.Top,
                    tiles.GetTile(viewport.Zoom, wrappedX, y, cancellationToken)));
            }
        }
        await Task.WhenAll(placements.Select(x => x.Data));
        cancellationToken.ThrowIfCancellationRequested();
        using var image = new Image<Rgba32>(Width, Height, Color.LightGray);
        foreach (var placement in placements)
        {
            using var tile = Image.Load<Rgba32>(await placement.Data);
            image.Mutate(x => x.DrawImage(tile, new Point(placement.X, placement.Y), 1));
        }
        var pixels = viewport.Points.Select(p => new PointF((float)(p.X * size - viewport.Left),
            (float)(p.Y * size - viewport.Top))).ToArray();
        image.Mutate(x =>
        {
            if (pixels.Length > 1) x.DrawLine(Color.RoyalBlue, 4, pixels);
            x.Fill(Color.White, new EllipsePolygon(pixels[0], 9));
            x.Fill(Color.ForestGreen, new EllipsePolygon(pixels[0], 7));
            if (pixels.Any(p => p != pixels[0]))
            {
                // A closed route keeps the green start visible around the red end.
                if (pixels[^1] == pixels[0]) x.Fill(Color.ForestGreen, new EllipsePolygon(pixels[0], 12));
                x.Fill(Color.White, new EllipsePolygon(pixels[^1], 9));
                x.Fill(Color.Crimson, new EllipsePolygon(pixels[^1], 7));
            }
            x.Fill(Color.White, new RectangleF(0, 0, Width, 32));
            x.DrawText($"{date.ToString("yyyy-MM-dd", CultureInfo.InvariantCulture)} · {timezone}", LabelFont.Value,
                Color.Black, new PointF(12, 6));
            const string attribution = "© OpenStreetMap contributors";
            var attributionFont = LabelFont.Value.Family.CreateFont(12);
            var attributionSize = TextMeasurer.MeasureAdvance(attribution, new TextOptions(attributionFont));
            var attributionBox = new RectangleF(Width - attributionSize.Width - 12,
                Height - attributionSize.Height - 8, attributionSize.Width + 8, attributionSize.Height + 4);
            x.Fill(Color.White.WithAlpha(0.85f), attributionBox);
            x.DrawText(attribution, attributionFont, Color.DimGray,
                new PointF(attributionBox.X + 4, attributionBox.Y + 2));
        });
        cancellationToken.ThrowIfCancellationRequested();
        using var output = new MemoryStream();
        await image.SaveAsPngAsync(output, cancellationToken);
        return output.ToArray();
    }

    internal static MapViewport GetViewport(MapCoordinate[] points)
    {
        // Cut the world at the largest longitude gap, keeping dateline crossings together.
        var longitudes = points.Select(p => ((p.Longitude + 180) / 360) % 1).Order().ToArray();
        var cut = longitudes[0];
        var largestGap = -1d;
        for (var i = 0; i < longitudes.Length; i++)
        {
            var next = i + 1 < longitudes.Length ? longitudes[i + 1] : longitudes[0] + 1;
            if (next - longitudes[i] > largestGap)
            {
                largestGap = next - longitudes[i];
                cut = longitudes[(i + 1) % longitudes.Length];
            }
        }
        var projected = points.Select(p =>
        {
            var x = ((p.Longitude + 180) / 360) % 1;
            if (x < cut) x += 1;
            var latitude = Math.Clamp(p.Latitude, -85.0511287798066, 85.0511287798066) * Math.PI / 180;
            var y = Math.Clamp((1 - Math.Log(Math.Tan(latitude) + 1 / Math.Cos(latitude)) / Math.PI) / 2, 0, 1);
            return new MapPoint(x, y);
        }).ToArray();
        var minX = projected.Min(p => p.X);
        var maxX = projected.Max(p => p.X);
        var minY = projected.Min(p => p.Y);
        var maxY = projected.Max(p => p.Y);
        var zoom = 18;
        while (zoom > 0 && ((maxX - minX) * 256 * (1 << zoom) > Width - 2 * Padding
            || (maxY - minY) * 256 * (1 << zoom) > Height - 2 * Padding)) zoom--;
        var size = 256 * (1 << zoom);
        return new MapViewport(zoom, (int)Math.Floor((minX + maxX) / 2 * size - Width / 2d),
            (int)Math.Floor((minY + maxY) / 2 * size - Height / 2d), projected);
    }

    internal readonly record struct MapCoordinate(double Latitude, double Longitude);
    internal readonly record struct MapPoint(double X, double Y);
    internal sealed record MapViewport(int Zoom, int Left, int Top, MapPoint[] Points);
}

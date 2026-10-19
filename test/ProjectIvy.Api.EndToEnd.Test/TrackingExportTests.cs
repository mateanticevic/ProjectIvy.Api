using System.Collections.Concurrent;
using System.Net;
using System.Net.Http.Headers;
using Microsoft.Extensions.Configuration;
using ProjectIvy.Business.Handlers.Tracking;
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.PixelFormats;
using Xunit;
using static ProjectIvy.Business.Handlers.Tracking.TrackingExportHandler;
using Tracking = ProjectIvy.Model.Database.Main.Tracking.Tracking;

namespace ProjectIvy.Api.EndToEnd.Test;

public sealed class TrackingExportTests
{
    [Theory]
    [InlineData("2026-03-29", "2026-03-28T23:00:00Z", 23)]
    [InlineData("2026-10-25", "2026-10-24T22:00:00Z", 25)]
    [InlineData("2026-01-15", "2026-01-14T23:00:00Z", 24)]
    public void ZagrebDayConvertsBothBoundaries(string date, string expectedStart, int hours)
    {
        var (from, to) = GetUtcBounds(DateOnly.Parse(date), "Europe/Zagreb");
        Assert.Equal(DateTime.Parse(expectedStart).ToUniversalTime(), from);
        Assert.Equal(hours, (to - from).TotalHours);
        Assert.Equal(DateTimeKind.Utc, from.Kind);
    }

    [Fact]
    public void MidnightClockChangesIncludeTheWholeLocalDay()
    {
        var (springStart, springEnd) = GetUtcBounds(new(2026, 3, 8), "America/Havana");
        Assert.Equal(new DateTime(2026, 3, 8, 5, 0, 0, DateTimeKind.Utc), springStart);
        Assert.Equal(23, (springEnd - springStart).TotalHours);
        var (fallStart, fallEnd) = GetUtcBounds(new(2026, 11, 1), "America/Havana");
        Assert.Equal(new DateTime(2026, 11, 1, 4, 0, 0, DateTimeKind.Utc), fallStart);
        Assert.Equal(25, (fallEnd - fallStart).TotalHours);
    }

    [Fact]
    public void CallerCanChooseTimezoneAndInvalidInputsAreRejected()
    {
        var date = new DateOnly(2026, 10, 19);
        Assert.Equal(new DateTime(2026, 10, 19, 4, 0, 0, DateTimeKind.Utc), GetUtcBounds(date, "America/New_York").From);
        Assert.Equal(date.ToDateTime(TimeOnly.MinValue, DateTimeKind.Utc), GetUtcBounds(date, "Etc/UTC").From);
        Assert.Equal(date.ToDateTime(TimeOnly.MinValue, DateTimeKind.Utc), GetUtcBounds(date, "UTC").From);
        Assert.Throws<ArgumentException>(() => GetUtcBounds(date, "unknown/zone"));
        Assert.Throws<ArgumentException>(() => GetUtcBounds(date, ""));
        Assert.Throws<ArgumentException>(() => GetUtcBounds(DateOnly.MaxValue, "Europe/Zagreb"));
    }

    [Fact]
    public void SelectionIsOwnedHalfOpenAndChronological()
    {
        var (from, to) = GetUtcBounds(new DateOnly(2026, 3, 29), "Europe/Zagreb");
        Tracking Point(int id, int user, DateTime time) => new() { Id = id, UserId = user, Timestamp = time };
        var points = new[] { Point(1, 7, to), Point(2, 7, from.AddHours(1)), Point(3, 7, from),
            Point(4, 8, from), Point(5, 7, from.AddTicks(-1)), Point(6, 7, to.AddTicks(-1)) };
        Assert.Equal(new[] { 3, 2, 6 }, SelectTrackings(points.AsQueryable(), 7, from, to).Select(p => p.Id));
    }

    [Theory]
    [InlineData(45, 179.99, 45, -179.99)]
    [InlineData(45, 180, 45, -180)]
    [InlineData(90, 0, -90, 0)]
    [InlineData(45.8, 15.9, 45.81, 15.91)]
    public void ViewportFitsWrappedAndPolarCoordinates(double lat1, double lon1, double lat2, double lon2)
    {
        var view = GetViewport([new(lat1, lon1), new(lat2, lon2)]);
        var size = 256 * (1 << view.Zoom);
        foreach (var point in view.Points)
        {
            Assert.InRange(point.X * size - view.Left, 47, Width - 47);
            Assert.InRange(point.Y * size - view.Top, 47, Height - 47);
        }
        if (lon1 > 179) Assert.True(view.Zoom >= 10);
    }

    [Fact]
    public async Task EmptyAndInvalidPointsDoNotFetchTiles()
    {
        using var tiles = new TileFixture();
        Assert.Null(await RenderMap([], new(2026, 10, 19), "Europe/Zagreb", tiles.Client, default));
        Assert.Null(await RenderMap([new(91, 0), new(0, -181), new(double.NaN, 0)],
            new(2026, 10, 19), "Europe/Zagreb", tiles.Client, default));
        Assert.Empty(tiles.Handler.Requests);
    }

    [Theory]
    [InlineData(false)]
    [InlineData(true)]
    public async Task StationaryAndSinglePointsRenderOneMarker(bool duplicate)
    {
        using var tiles = new TileFixture();
        MapCoordinate point = new(45.8, 16);
        var png = await RenderMap(duplicate ? [point, point] : [point], new(2026, 10, 19),
            "Europe/Zagreb", tiles.Client, default);
        using var image = Image.Load<Rgba32>(png);
        Assert.Equal(Color.ForestGreen.ToPixel<Rgba32>(), image[512, 384]);
        Assert.DoesNotContain(Enumerable.Range(0, Width).SelectMany(x => Enumerable.Range(32, Height - 62)
            .Select(y => image[x, y])), pixel => pixel == Color.Crimson.ToPixel<Rgba32>());
    }

    [Fact]
    public async Task RendersPngRouteMarkersAndLabelsWithFakeTiles()
    {
        using var tiles = new TileFixture();
        MapCoordinate[] points = [new(45.8, 15.9), new(45.81, 15.93), new(45.82, 15.94)];
        var png = await RenderMap(points, new(2026, 10, 19), "Europe/Zagreb", tiles.Client, default);
        using var image = Image.Load<Rgba32>(png);
        Assert.Equal((Width, Height), (image.Width, image.Height));
        Assert.Equal("PNG", Image.DetectFormat(png).Name);
        var view = GetViewport(points);
        var size = 256 * (1 << view.Zoom);
        Rgba32 Pixel(MapPoint p) => image[(int)Math.Round(p.X * size - view.Left), (int)Math.Round(p.Y * size - view.Top)];
        Assert.Equal(Color.ForestGreen.ToPixel<Rgba32>(), Pixel(view.Points[0]));
        Assert.Equal(Color.RoyalBlue.ToPixel<Rgba32>(), Pixel(view.Points[1]));
        Assert.Equal(Color.Crimson.ToPixel<Rgba32>(), Pixel(view.Points[^1]));
        Assert.Equal(Color.Beige.ToPixel<Rgba32>(), image[12, Height - 12]);
        Assert.Contains(Enumerable.Range(Width - 240, 236).SelectMany(x => Enumerable.Range(Height - 28, 24)
            .Select(y => image[x, y])), pixel => pixel.R < 150 && pixel.G < 150 && pixel.B < 150);
        Assert.Contains(Enumerable.Range(12, 250).SelectMany(x => Enumerable.Range(6, 22)
            .Select(y => image[x, y])), pixel => pixel.R < 100 && pixel.G < 100 && pixel.B < 100);
        Assert.InRange(tiles.Handler.Requests.Count, 1, 20);
        Assert.All(tiles.Handler.Requests, r => Assert.Contains("ProjectIvy/1.0", r.UserAgent));
    }

    [Fact]
    public async Task ClosedRouteKeepsBothMarkersVisible()
    {
        using var tiles = new TileFixture();
        MapCoordinate[] points = [new(45.8, 16), new(45.81, 16.01), new(45.8, 16)];
        var png = await RenderMap(points, new(2026, 10, 19), "Europe/Zagreb", tiles.Client, default);
        using var image = Image.Load<Rgba32>(png);
        var view = GetViewport(points);
        var size = 256 * (1 << view.Zoom);
        var start = view.Points[0];
        var x = (int)Math.Round(start.X * size - view.Left);
        var y = (int)Math.Round(start.Y * size - view.Top);
        Assert.Equal(Color.Crimson.ToPixel<Rgba32>(), image[x, y]);
        Assert.Equal(Color.ForestGreen.ToPixel<Rgba32>(), image[x - 10, y]);
    }

    [Fact]
    public async Task DiskCacheCoalescesDownloadsAndSurvivesNewClient()
    {
        using var tiles = new TileFixture();
        var results = await Task.WhenAll(Enumerable.Range(0, 8).Select(_ => tiles.Client.GetTile(5, 1, 2, default)));
        Assert.Single(tiles.Handler.Requests);
        Assert.All(results, result => Assert.Equal(results[0], result));
        using var secondClient = tiles.NewClient();
        Assert.Equal(results[0], await secondClient.GetTile(5, 1, 2, default));
        Assert.Single(tiles.Handler.Requests);
    }

    [Fact]
    public async Task ExpiredTilesRevalidateWithEtagAndLastModified()
    {
        using var tiles = new TileFixture();
        tiles.Handler.Response = request => request.Headers.IfNoneMatch.Count > 0
            ? new(HttpStatusCode.NotModified) { Headers = { CacheControl = new() { MaxAge = TimeSpan.FromDays(1) } } }
            : new(HttpStatusCode.OK)
            {
                Content = new ByteArrayContent(TileFixture.Tile)
                { Headers = { LastModified = new DateTimeOffset(2026, 1, 1, 0, 0, 0, TimeSpan.Zero) } },
                Headers = { ETag = new EntityTagHeaderValue("\"tile-v1\""), CacheControl = new() { MaxAge = TimeSpan.Zero } }
            };
        var first = await tiles.Client.GetTile(5, 1, 2, default);
        Assert.Equal(first, await tiles.Client.GetTile(5, 1, 2, default));
        Assert.Equal(first, await tiles.Client.GetTile(5, 1, 2, default));
        Assert.Equal(2, tiles.Handler.Requests.Count);
        Assert.Equal("\"tile-v1\"", tiles.Handler.Requests.Last().ETag);
        Assert.Equal(new DateTimeOffset(2026, 1, 1, 0, 0, 0, TimeSpan.Zero), tiles.Handler.Requests.Last().LastModified);
    }

    [Fact]
    public async Task NotModifiedPreservesRevalidationPolicyWhenHeadersAreOmitted()
    {
        using var tiles = new TileFixture();
        tiles.Handler.Response = request => request.Headers.IfNoneMatch.Count > 0
            ? new(HttpStatusCode.NotModified)
            : new(HttpStatusCode.OK)
            {
                Content = new ByteArrayContent(TileFixture.Tile),
                Headers = { ETag = new EntityTagHeaderValue("\"tile\""), CacheControl = new() { NoCache = true } }
            };
        await tiles.Client.GetTile(5, 1, 2, default);
        await tiles.Client.GetTile(5, 1, 2, default);
        await tiles.Client.GetTile(5, 1, 2, default);
        Assert.Equal(3, tiles.Handler.Requests.Count);
    }

    [Fact]
    public async Task NoStoreAndFailedDownloadsAreNotCached()
    {
        using var tiles = new TileFixture();
        tiles.Handler.Response = _ => new(HttpStatusCode.OK)
        {
            Content = new ByteArrayContent(TileFixture.Tile), Headers = { CacheControl = new() { NoStore = true } }
        };
        await tiles.Client.GetTile(5, 1, 2, default);
        await tiles.Client.GetTile(5, 1, 2, default);
        Assert.Equal(2, tiles.Handler.Requests.Count);
        tiles.Handler.Response = _ => new(HttpStatusCode.ServiceUnavailable);
        await Assert.ThrowsAsync<HttpRequestException>(() => tiles.Client.GetTile(5, 1, 2, default));
        tiles.Handler.Response = _ => new(HttpStatusCode.OK) { Content = new StringContent("not an image") };
        await Assert.ThrowsAsync<UnknownImageFormatException>(() => tiles.Client.GetTile(5, 1, 2, default));
    }

    [Fact]
    public async Task DownloadsAreBoundedAndCancellationReleasesLocks()
    {
        using var tiles = new TileFixture();
        tiles.Handler.Delay = TimeSpan.FromMilliseconds(30);
        await Task.WhenAll(Enumerable.Range(0, 12).Select(x => tiles.Client.GetTile(5, x, 2, default)));
        Assert.InRange(tiles.Handler.MaximumConcurrency, 2, 4);
        using var cancellation = new CancellationTokenSource();
        tiles.Handler.Delay = TimeSpan.FromSeconds(30);
        var request = tiles.Client.GetTile(5, 20, 2, cancellation.Token);
        cancellation.Cancel();
        await Assert.ThrowsAnyAsync<OperationCanceledException>(() => request);
        tiles.Handler.Delay = TimeSpan.Zero;
        await tiles.Client.GetTile(5, 20, 2, default);
    }

    internal sealed class TileFixture : IDisposable, IHttpClientFactory
    {
        internal static byte[] Tile { get; } = MakeTile();
        private readonly string _directory = Path.Combine(Path.GetTempPath(), "ivy-map-test-" + Guid.NewGuid().ToString("N"));
        internal FakeTileHandler Handler { get; } = new();
        internal TrackingMapTileClient Client { get; }
        internal TileFixture() => Client = NewClient();
        internal TrackingMapTileClient NewClient() => new(this, new ConfigurationBuilder().AddInMemoryCollection(
            new Dictionary<string, string?> { ["TrackingExport:TileUrl"] = "https://tiles.example.test/{z}/{x}/{y}.png",
                ["TrackingExport:CacheDirectory"] = _directory }).Build());
        public HttpClient CreateClient(string name) => new(Handler, disposeHandler: false);
        private static byte[] MakeTile()
        {
            using var image = new Image<Rgba32>(256, 256, Color.Beige);
            using var output = new MemoryStream();
            image.SaveAsPng(output);
            return output.ToArray();
        }
        public void Dispose()
        {
            Client.Dispose();
            Handler.Dispose();
            if (Directory.Exists(_directory)) Directory.Delete(_directory, recursive: true);
        }
    }

    internal sealed class FakeTileHandler : HttpMessageHandler
    {
        internal ConcurrentQueue<(string UserAgent, string? ETag, DateTimeOffset? LastModified)> Requests { get; } = new();
        internal Func<HttpRequestMessage, HttpResponseMessage> Response { get; set; }
            = _ => new(HttpStatusCode.OK) { Content = new ByteArrayContent(TileFixture.Tile) };
        internal TimeSpan Delay { get; set; }
        internal int MaximumConcurrency;
        private int _active;
        protected override async Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
        {
            Requests.Enqueue((request.Headers.UserAgent.ToString(), request.Headers.IfNoneMatch.FirstOrDefault()?.ToString(), request.Headers.IfModifiedSince));
            var active = Interlocked.Increment(ref _active);
            int observed;
            do { observed = MaximumConcurrency; }
            while (active > observed && Interlocked.CompareExchange(ref MaximumConcurrency, active, observed) != observed);
            try
            {
                await Task.Delay(Delay, cancellationToken);
                return Response(request);
            }
            finally { Interlocked.Decrement(ref _active); }
        }
    }
}

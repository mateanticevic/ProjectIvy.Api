using System.Security.Claims;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging.Abstractions;
using ProjectIvy.Api.EndToEnd.Test.Infrastructure;
using ProjectIvy.Business.Handlers;
using ProjectIvy.Business.Handlers.Tracking;
using SixLabors.ImageSharp;
using Xunit;
using static ProjectIvy.Business.Handlers.Tracking.TrackingExportHandler;
using Tracking = ProjectIvy.Model.Database.Main.Tracking.Tracking;

namespace ProjectIvy.Api.EndToEnd.Test;

[Collection(SqlServerCollection.Name)]
public sealed class TrackingExportDatabaseTests(SqlServerFixture fixture) : IAsyncLifetime
{
    private readonly ExpenseTestSession _session = new(fixture);
    public Task InitializeAsync() => _session.InitializeAsync();
    public async Task DisposeAsync() => await _session.DisposeAsync();

    [Fact]
    public async Task OmittedTimezoneUsesAuthenticatedUsersDatabaseDefaultAndAllowsOverride()
    {
        using var db = _session.OpenDatabase();
        var user = await db.Users.SingleAsync(x => x.Id == _session.User.Id);
        user.DefaultTimezone = "America/New_York";
        var other = await db.Users.SingleAsync(x => x.Id == _session.OtherUser.Id);
        other.DefaultTimezone = "Asia/Tokyo";
        db.Trackings.AddRange(
            new Tracking { UserId = user.Id, Timestamp = new(2026, 10, 19, 0, 0, 0), Latitude = 45.8m, Longitude = 20, Geohash = "u25" },
            new Tracking { UserId = user.Id, Timestamp = new(2026, 10, 19, 4, 0, 0), Latitude = 45.8m, Longitude = 16, Geohash = "u25" },
            new Tracking { UserId = user.Id, Timestamp = new(2026, 10, 20, 2, 0, 0), Latitude = 45.81m, Longitude = 16.05m, Geohash = "u25" });
        await db.SaveChangesAsync();
        using var tiles = new TrackingExportTests.TileFixture();
        var handler = CreateHandler(tiles.Client);
        var date = new DateOnly(2026, 10, 19);
        var implicitMap = await handler.GetMap(date);
        var explicitMap = await handler.GetMap(date, "America/New_York");
        Assert.Equal(explicitMap, implicitMap);
        var overrideMap = await handler.GetMap(date, "Europe/Zagreb");
        Assert.False(implicitMap.SequenceEqual(overrideMap));

        user.DefaultTimezone = "";
        await db.SaveChangesAsync();
        await Assert.ThrowsAsync<ArgumentException>(() => handler.GetMap(date));
        var mapWithoutDefault = await handler.GetMap(date, "America/New_York");
        Assert.Equal(explicitMap, mapWithoutDefault);
    }

    [Fact]
    public async Task SqlSelectionUsesLocalDayBoundariesOwnerAndChronologicalOrder()
    {
        var date = new DateOnly(2026, 3, 29);
        var (from, to) = GetUtcBounds(date, "Europe/Zagreb");
        Tracking Point(int owner, DateTime timestamp, decimal longitude) => new()
        {
            UserId = owner, Timestamp = timestamp, Latitude = 45.8m,
            Longitude = longitude, Geohash = "u25"
        };
        using var db = _session.OpenDatabase();
        db.Trackings.AddRange(
            Point(_session.User.Id, to.AddMilliseconds(-1), 16.02m),
            Point(_session.User.Id, to, 17),
            Point(_session.OtherUser.Id, from.AddHours(1), 18),
            Point(_session.User.Id, from, 16),
            Point(_session.User.Id, from.AddMilliseconds(-1), 19),
            Point(_session.User.Id, from.AddHours(1), 16.01m));
        await db.SaveChangesAsync();
        var points = await SelectTrackings(db.Trackings.AsNoTracking(), _session.User.Id, from, to)
            .Select(x => new MapCoordinate((double)x.Latitude, (double)x.Longitude)).ToListAsync();
        Assert.Equal(new[] { 16d, 16.01, 16.02 }, points.Select(p => p.Longitude));
        using var tiles = new TrackingExportTests.TileFixture();
        var handler = CreateHandler(tiles.Client);
        var png = await handler.GetMap(date, "Europe/Zagreb");
        using var image = Image.Load(png);
        Assert.Equal((1024, 768), (image.Width, image.Height));
        Assert.Null(await handler.GetMap(date.AddDays(-2), "Europe/Zagreb"));
        using var cancellation = new CancellationTokenSource();
        cancellation.Cancel();
        await Assert.ThrowsAnyAsync<OperationCanceledException>(() => handler.GetMap(date, "Europe/Zagreb", cancellation.Token));
        _session.Factory!.VerifyNoExternalCalls();
    }

    private TrackingExportHandler CreateHandler(TrackingMapTileClient tiles)
    {
        var httpContext = new DefaultHttpContext
        {
            User = new ClaimsPrincipal(new ClaimsIdentity([new Claim(ClaimTypes.Email, _session.User.Email)], "Test"))
        };
        return new TrackingExportHandler(new HandlerContext<TrackingExportHandler>(
            new HttpContextAccessor { HttpContext = httpContext }, NullLogger<TrackingExportHandler>.Instance), tiles);
    }
}

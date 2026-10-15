using System.Net;
using System.Net.Http.Json;
using System.Text.Json;
using Microsoft.Data.SqlClient;
using ProjectIvy.Api.Constants;
using ProjectIvy.Api.EndToEnd.Test.Infrastructure;
using ProjectIvy.Model.Database.Main.Transport;
using ProjectIvy.Model.Database.Main.Travel;
using Xunit;
using Tracking = ProjectIvy.Model.Database.Main.Tracking.Tracking;

namespace ProjectIvy.Api.EndToEnd.Test;

[Collection(SqlServerCollection.Name)]
public sealed class LastTrackingTests(SqlServerFixture fixture) : IAsyncLifetime
{
    private readonly ExpenseTestSession _session = new(fixture);

    public Task InitializeAsync() => _session.InitializeAsync();
    public async Task DisposeAsync() => await _session.DisposeAsync();

    [Fact]
    public async Task CutoffIsExclusiveAndOnlyReturnsOwnersTracking()
    {
        var at = new DateTime(2020, 1, 15, 12, 0, 0, DateTimeKind.Utc);
        using (var db = _session.OpenDatabase())
        {
            db.Trackings.AddRange(
                NewTracking(_session.User.Id, at.AddMinutes(-2)),
                NewTracking(_session.User.Id, at.AddMinutes(-1)),
                NewTracking(_session.User.Id, at),
                NewTracking(_session.User.Id, at.AddMinutes(1)),
                NewTracking(_session.OtherUser.Id, at.AddSeconds(-1)),
                NewTracking(_session.OtherUser.Id, at.AddMinutes(2)));
            await db.SaveChangesAsync();
        }

        using var client = _session.Factory!.CreateUserClient(_session.User.Email, ApiScopes.TrackingViewLast);
        var historical = (await client.GetFromJsonAsync<JsonElement>(
            $"/Tracking/lastLocation?at={at:yyyy-MM-ddTHH:mm:ssZ}"))!;
        Assert.Equal(at.AddMinutes(-1), DateTime.Parse(historical.GetProperty("tracking").GetProperty("timestamp").GetString()!));
        var latest = (await client.GetFromJsonAsync<JsonElement>("/Tracking/lastLocation"))!;
        Assert.Equal(at.AddMinutes(1), DateTime.Parse(latest.GetProperty("tracking").GetProperty("timestamp").GetString()!));
        Assert.Equal(HttpStatusCode.NotFound,
            (await client.GetAsync($"/Tracking/lastLocation?at={at.AddMinutes(-2):yyyy-MM-ddTHH:mm:ssZ}")).StatusCode);
    }

    [Fact]
    public async Task NoOwnedTrackingReturnsNotFound()
    {
        using (var db = _session.OpenDatabase())
        {
            db.Trackings.Add(NewTracking(_session.OtherUser.Id, DateTime.UtcNow.AddDays(-1)));
            await db.SaveChangesAsync();
        }

        using var client = _session.Factory!.CreateUserClient(_session.User.Email, ApiScopes.TrackingViewLast);
        Assert.Equal(HttpStatusCode.NotFound, (await client.GetAsync("/Tracking/lastLocation")).StatusCode);
        Assert.Equal(HttpStatusCode.NotFound,
            (await client.GetAsync("/Tracking/lastLocation?at=2026-10-15T12:00:00Z")).StatusCode);
    }

    [Fact]
    public async Task FlightUsesRequestedTime()
    {
        var at = new DateTime(2020, 1, 15, 12, 0, 0, DateTimeKind.Utc);
        using (var db = _session.OpenDatabase())
        {
            var poi = new Poi
            {
                Name = "Test airport", ValueId = "test-airport", Address = "Test address",
                Created = at, Modified = at, Latitude = 45, Longitude = 16,
                PoiCategory = new PoiCategory { Name = "Airport", ValueId = "airport" }
            };
            db.Add(poi);
            await db.SaveChangesAsync();

            // Country codes and City.PlaceTypeId are required by the snapshot but absent from the EF model.
            await using var connection = new SqlConnection(_session.ConnectionString);
            await connection.OpenAsync();
            await using var command = connection.CreateCommand();
            command.CommandText = """
                INSERT INTO [Common].[Country] ([ValueId], [Name], [TwoLetterCode], [ThreeLetterCode])
                VALUES ('test-country', 'Test country', 'ZZ', 'ZZZ');
                DECLARE @countryId int = SCOPE_IDENTITY();
                INSERT INTO [Common].[City] ([ValueId], [Name], [CountryId], [PlaceTypeId])
                VALUES ('test-city', 'Test city', @countryId, 1);
                DECLARE @cityId int = SCOPE_IDENTITY();
                INSERT INTO [Transport].[Airport] ([IATA], [Name], [PoiId], [CityId])
                OUTPUT INSERTED.[Id] VALUES ('TST', 'Test airport', @poiId, @cityId)
                """;
            command.Parameters.AddWithValue("@poiId", poi.Id);
            var airportId = Convert.ToInt32(await command.ExecuteScalarAsync());
            db.Flights.Add(new Flight
            {
                UserId = _session.User.Id, ValueId = "past-flight", FlightNumber = "TST1",
                Airline = new Airline { Name = "Test airline", ValueId = "test-airline" },
                OriginAirportId = airportId, DestinationAirportId = airportId,
                DateOfDeparture = at.AddHours(-1), DateOfDepartureLocal = at.AddHours(-1),
                DateOfArrival = at.AddHours(1), DateOfArrivalLocal = at.AddHours(1)
            });
            db.Trackings.Add(NewTracking(_session.User.Id, at.AddMinutes(-1)));
            await db.SaveChangesAsync();
        }

        using var client = _session.Factory!.CreateUserClient(_session.User.Email, ApiScopes.TrackingViewLast);
        var historical = (await client.GetFromJsonAsync<JsonElement>(
            $"/Tracking/lastLocation?at={at:yyyy-MM-ddTHH:mm:ssZ}"))!;
        Assert.Equal("past-flight", historical.GetProperty("flight").GetProperty("id").GetString());
        var latest = (await client.GetFromJsonAsync<JsonElement>("/Tracking/lastLocation"))!;
        Assert.False(latest.TryGetProperty("flight", out _));
    }

    private static Tracking NewTracking(int userId, DateTime timestamp) => new()
    {
        UserId = userId, Timestamp = timestamp, Latitude = 45, Longitude = 16, Geohash = "u25"
    };
}

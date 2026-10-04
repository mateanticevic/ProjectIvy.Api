using System.Net;
using System.Net.Http.Json;
using System.Text.Json;
using ProjectIvy.Model.Converters;
using ProjectIvy.Api.EndToEnd.Test.Infrastructure;
using ProjectIvy.Model.Binding.Tracking;
using Xunit;
using View = ProjectIvy.Model.View.Tracking.TrackingView;

namespace ProjectIvy.Api.EndToEnd.Test;

[Collection(SqlServerCollection.Name)]
public sealed class TrackingViewTests(SqlServerFixture fixture) : IAsyncLifetime
{
    private readonly ExpenseTestSession _session = new(fixture);
    private static readonly JsonSerializerOptions JsonOptions = new(JsonSerializerDefaults.Web)
    {
        Converters = { new CustomDateTimeConverter() }
    };
    public Task InitializeAsync() => _session.InitializeAsync();
    public async Task DisposeAsync() => await _session.DisposeAsync();

    [Fact]
    public async Task CrudIsOwnedAndPublicSharingReflectsChanges()
    {
        using var owner = _session.Factory!.CreateUserClient(_session.User.Email);
        using var other = _session.Factory.CreateUserClient(_session.OtherUser.Email);
        using var anonymous = _session.Factory.CreateUserClient(null, null);
        Assert.Equal(HttpStatusCode.Unauthorized, (await anonymous.GetAsync("/TrackingView")).StatusCode);
        var date = new DateTime(2026, 10, 4);
        var binding = new TrackingViewBinding { Name = "Morning Walk", From = date, To = date };
        var created = await owner.PostAsJsonAsync("/TrackingView", binding);
        Assert.True(created.StatusCode == HttpStatusCode.Created, await created.Content.ReadAsStringAsync());
        var view = (await created.Content.ReadFromJsonAsync<View>(JsonOptions))!;
        Assert.Equal("morning-walk", view.Id);
        Assert.Equal(HttpStatusCode.OK, (await owner.GetAsync(created.Headers.Location)).StatusCode);
        Assert.Single((await owner.GetFromJsonAsync<View[]>("/TrackingView", JsonOptions))!);
        Assert.Empty((await other.GetFromJsonAsync<View[]>("/TrackingView", JsonOptions))!);
        var path = $"/TrackingView/{view.Id}";
        Assert.Equal(HttpStatusCode.NotFound, (await other.GetAsync(path)).StatusCode);
        Assert.Equal(HttpStatusCode.NotFound, (await other.PutAsJsonAsync(path, binding)).StatusCode);
        Assert.Equal(HttpStatusCode.NotFound, (await other.DeleteAsync(path)).StatusCode);
        Assert.Equal(HttpStatusCode.BadRequest, (await owner.PostAsJsonAsync("/TrackingView", binding)).StatusCode);
        Assert.Equal(HttpStatusCode.Created, (await other.PostAsJsonAsync("/TrackingView", binding)).StatusCode);
        var publicPath = $"/user/{_session.User.Username}/trackingview/{view.Id}";
        Assert.Equal(HttpStatusCode.OK, (await anonymous.GetAsync(publicPath)).StatusCode);
        binding.Name = "Renamed";
        binding.To = date.AddDays(1);
        Assert.Equal(HttpStatusCode.NoContent, (await owner.PutAsJsonAsync(path, binding)).StatusCode);
        var updated = (await owner.GetFromJsonAsync<View>(path, JsonOptions))!;
        Assert.Equal(view.Id, updated.Id);
        Assert.Equal("Renamed", updated.Name);
        Assert.Equal(binding.To, updated.To);
        Assert.Equal(HttpStatusCode.OK, (await anonymous.GetAsync(publicPath)).StatusCode);
        Assert.Equal(HttpStatusCode.BadRequest, (await owner.PostAsJsonAsync("/TrackingView", new { Name = "Missing dates" })).StatusCode);
        Assert.Equal(HttpStatusCode.NoContent, (await owner.DeleteAsync(path)).StatusCode);
        Assert.Equal(HttpStatusCode.NotFound, (await owner.GetAsync(path)).StatusCode);
        Assert.Equal(HttpStatusCode.NotFound, (await owner.PutAsJsonAsync(path, binding)).StatusCode);
        Assert.Equal(HttpStatusCode.NotFound, (await owner.DeleteAsync(path)).StatusCode);
        Assert.Equal(HttpStatusCode.NotFound, (await anonymous.GetAsync(publicPath)).StatusCode);
        _session.Factory.VerifyNoExternalCalls();
    }
}

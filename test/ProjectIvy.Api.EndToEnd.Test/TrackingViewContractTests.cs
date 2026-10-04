using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using ProjectIvy.Api.Controllers.Tracking;
using ProjectIvy.Business.Exceptions;
using ProjectIvy.Business.MapExtensions;
using ProjectIvy.Model.Binding.Tracking;
using ProjectIvy.Model.Database.Main.Tracking;
using Xunit;

namespace ProjectIvy.Api.EndToEnd.Test;

public class TrackingViewContractTests
{
    [Fact]
    public void CrudRequiresAuthenticationAndPublicRouteRemainsAnonymous()
    {
        Assert.Single(typeof(TrackingViewController).GetCustomAttributes(typeof(AuthorizeAttribute), true));
        Assert.Empty(typeof(TrackingViewController).GetCustomAttributes(typeof(AllowAnonymousAttribute), true));
        Assert.Single(typeof(UserTrackingViewController).GetCustomAttributes(typeof(AllowAnonymousAttribute), true));
        var method = typeof(UserTrackingViewController).GetMethod("Get")!;
        var route = Assert.Single(method.GetCustomAttributes(typeof(HttpGetAttribute), true).Cast<HttpGetAttribute>());
        Assert.Equal("/user/{username}/trackingview/{viewIdentifier}", route.Template);
    }

    [Theory]
    [InlineData(null, true, true, false)]
    [InlineData(" ", true, true, false)]
    [InlineData("#", true, true, false)]
    [InlineData("View", false, true, false)]
    [InlineData("View", true, false, false)]
    [InlineData("View", true, true, true)]
    public void InvalidBindingsAreRejected(string? name, bool hasFrom, bool hasTo, bool reversed)
    {
        var date = new DateTime(2026, 10, 4);
        var binding = new TrackingViewBinding
        {
            Name = name!, From = hasFrom ? date : null,
            To = hasTo ? date.AddDays(reversed ? -1 : 0) : null
        };
        Assert.Throws<InvalidRequestException>(() => binding.ToEntity());
    }

    [Fact]
    public void CreateGeneratesIdAndAcceptsEqualBoundaries()
    {
        var date = new DateTime(2026, 10, 4);
        var entity = new TrackingViewBinding { Name = "Morning Walk", From = date, To = date }.ToEntity();
        Assert.Equal("morning-walk", entity.ValueId);
        Assert.Equal(date, entity.From);
        Assert.Equal(date, entity.To);
    }

    [Fact]
    public void UpdatePreservesSharingIdAndOwner()
    {
        var entity = new TrackingView { ValueId = "original", UserId = 42 };
        var date = new DateTime(2026, 10, 4);
        var result = new TrackingViewBinding { Name = "Renamed", From = date, To = date.AddDays(1) }.ToEntity(entity);
        Assert.Same(entity, result);
        Assert.Equal("original", result.ValueId);
        Assert.Equal(42, result.UserId);
        Assert.Equal("Renamed", result.Name);
        Assert.Equal(date.AddDays(1), result.To);
    }
}

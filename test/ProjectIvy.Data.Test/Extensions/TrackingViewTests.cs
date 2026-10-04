using Microsoft.EntityFrameworkCore;
using ProjectIvy.Data.DbContexts;
using ProjectIvy.Data.Extensions;
using ProjectIvy.Model.Database.Main.Tracking;

namespace ProjectIvy.Data.Test.Extensions;

public class TrackingViewTests
{
    [Fact]
    public void TrackingViewMapsToTrackingSchemaWithMillisecondDatesAndOwner()
    {
        using var db = new MainContext("Server=localhost;Database=ModelOnly;Integrated Security=true");
        var entity = db.Model.FindEntityType(typeof(TrackingView))!;

        Assert.Equal("Tracking", entity.GetSchema());
        Assert.Equal("TrackingView", entity.GetTableName());
        Assert.Equal("datetime2(3)", entity.FindProperty(nameof(TrackingView.From))!.GetColumnType());
        Assert.Equal("datetime2(3)", entity.FindProperty(nameof(TrackingView.To))!.GetColumnType());
        Assert.Contains(entity.GetForeignKeys(), key =>
            key.Properties.Single().Name == nameof(TrackingView.UserId)
            && key.PrincipalEntityType.ClrType == typeof(Model.Database.Main.User.User));
    }

    [Fact]
    public void SavedRangeIncludesBothBoundariesAndOnlyOwnersTrackings()
    {
        var from = new DateTime(2026, 10, 4, 12, 0, 0, 123);
        var to = from.AddMilliseconds(2);
        var trackings = new[]
        {
            new Tracking { UserId = 1, Timestamp = from.AddMilliseconds(-1) },
            new Tracking { UserId = 1, Timestamp = from },
            new Tracking { UserId = 1, Timestamp = to },
            new Tracking { UserId = 1, Timestamp = to.AddMilliseconds(1) },
            new Tracking { UserId = 2, Timestamp = from }
        };

        var result = trackings.AsQueryable().WhereUser(1).WhereTimestampInclusive(from, to).ToList();

        Assert.Equal(new[] { from, to }, result.Select(x => x.Timestamp));
        Assert.All(result, x => Assert.Equal(1, x.UserId));
    }
}

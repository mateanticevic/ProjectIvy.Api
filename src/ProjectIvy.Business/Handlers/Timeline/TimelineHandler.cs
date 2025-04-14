using Dapper;
using Microsoft.EntityFrameworkCore;
using ProjectIvy.Data.Extensions;
using ProjectIvy.Data.Sql;
using ProjectIvy.Data.Sql.Main.Scripts;
using ProjectIvy.Model.Binding.Timeline;
using ProjectIvy.Model.View;
using System.Linq;
using System.Threading.Tasks;
using View = ProjectIvy.Model.View.Timeline;

namespace ProjectIvy.Business.Handlers.Timeline;

public class TimelineHandler : Handler<TimelineHandler>, ITimelineHandler
{
    public TimelineHandler(IHandlerContext<TimelineHandler> context) : base(context)
    {
    }

    public async Task<IEnumerable<View.TimelineItem>> Get(TimelineGetBinding binding)
    {
        var sqlConnection = GetSqlConnection();

        var parameters = new
        {
            From = binding.From,
            To = binding.To,
            UserId = UserId
        };

        var locations = await sqlConnection.QueryAsync<(int locationId, DateTime entry, DateTime exit)>(SqlLoader.Load(SqlScripts.GetLocationsFromTrackings), parameters);

        return null;
    }
}
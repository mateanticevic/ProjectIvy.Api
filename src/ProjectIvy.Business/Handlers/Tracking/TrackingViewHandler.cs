using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using ProjectIvy.Business.Exceptions;
using ProjectIvy.Data.Extensions;
using View = ProjectIvy.Model.View.Tracking;

namespace ProjectIvy.Business.Handlers.Tracking;

public class TrackingViewHandler : Handler<TrackingViewHandler>, ITrackingViewHandler
{
    public TrackingViewHandler(IHandlerContext<TrackingViewHandler> context)
        : base(context, requireAuthentication: false)
    {
    }

    public async Task<IEnumerable<View.Tracking>> Get(string username, string viewIdentifier)
    {
        using var db = GetMainContext();
        var userId = await db.Users.Where(x => x.Username == username)
                                   .Select(x => (int?)x.Id)
                                   .SingleOrDefaultAsync();
        if (!userId.HasValue)
            throw new ResourceNotFoundException();

        var view = await db.TrackingViews.AsNoTracking()
                                        .WhereUser(userId.Value)
                                        .SingleOrDefaultAsync(x => x.ValueId == viewIdentifier) ?? throw new ResourceNotFoundException();
                                        
        return (await db.Trackings.AsNoTracking()
                                 .WhereUser(userId.Value)
                                 .WhereTimestampInclusive(view.From, view.To)
                                 .OrderBy(x => x.Timestamp)
                                 .ToListAsync())
                                 .Select(x => new View.Tracking(x)).ToList();
    }
}

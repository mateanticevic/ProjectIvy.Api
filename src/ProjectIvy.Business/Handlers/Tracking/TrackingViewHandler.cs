using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using ProjectIvy.Business.Exceptions;
using ProjectIvy.Business.MapExtensions;
using ProjectIvy.Data.Extensions;
using ProjectIvy.Model.Binding.Tracking;
using View = ProjectIvy.Model.View.Tracking;

namespace ProjectIvy.Business.Handlers.Tracking;

public class TrackingViewHandler : Handler<TrackingViewHandler>, ITrackingViewHandler
{
    public TrackingViewHandler(IHandlerContext<TrackingViewHandler> context) : base(context) { }

    public async Task<IEnumerable<View.TrackingView>> Get()
    {
        using var db = GetMainContext();
        return await db.TrackingViews.AsNoTracking().WhereUser(UserId)
            .OrderBy(x => x.Name).ThenBy(x => x.ValueId)
            .Select(x => new View.TrackingView(x)).ToListAsync();
    }

    public async Task<View.TrackingView> GetSingle(string id)
    {
        using var db = GetMainContext();
        var entity = await db.TrackingViews.AsNoTracking().WhereUser(UserId)
            .SingleOrDefaultAsync(x => x.ValueId == id) ?? throw new ResourceNotFoundException();
        return new View.TrackingView(entity);
    }

    public async Task<View.TrackingView> Create(TrackingViewBinding binding)
    {
        var entity = binding.ToEntity();
        entity.UserId = UserId;
        using var db = GetMainContext();
        if (await db.TrackingViews.WhereUser(UserId).AnyAsync(x => x.ValueId == entity.ValueId))
            throw new InvalidRequestException("A tracking view with this ID already exists.");
        await db.TrackingViews.AddAsync(entity);
        await db.SaveChangesAsync();
        return new View.TrackingView(entity);
    }

    public async Task Update(string id, TrackingViewBinding binding)
    {
        using var db = GetMainContext();
        var entity = await db.TrackingViews.WhereUser(UserId)
            .SingleOrDefaultAsync(x => x.ValueId == id) ?? throw new ResourceNotFoundException();
        binding.ToEntity(entity);
        await db.SaveChangesAsync();
    }

    public async Task Delete(string id)
    {
        using var db = GetMainContext();
        var entity = await db.TrackingViews.WhereUser(UserId)
            .SingleOrDefaultAsync(x => x.ValueId == id) ?? throw new ResourceNotFoundException();
        db.TrackingViews.Remove(entity);
        await db.SaveChangesAsync();
    }
}

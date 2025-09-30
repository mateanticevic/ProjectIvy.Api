using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using ProjectIvy.Data.Extensions;
using ProjectIvy.Data.Extensions.Entities;
using ProjectIvy.Model.Binding.Stay;
using ProjectIvy.Model.View;
using Database = ProjectIvy.Model.Database.Main;
using View = ProjectIvy.Model.View;

namespace ProjectIvy.Business.Handlers.Stay;

public class StayHandler : Handler<StayHandler>, IStayHandler
{
    public StayHandler(IHandlerContext<StayHandler> context) : base(context)
    {
    }

    public async Task AddStay(DateTime date, string cityValueId, string countryValueId)
    {
        using var context = GetMainContext();
        
        int? cityId = null;
        if (!string.IsNullOrEmpty(cityValueId))
            cityId = context.Cities.GetId(cityValueId);

        int countryId = context.Countries.GetId(countryValueId).Value;
        
        var stay = new Database.Travel.Stay()
        {   
            Date = date,
            CityId = cityId,
            CountryId = countryId,
            UserId = UserId
        };
        
        context.Stays.Add(stay);
        await context.SaveChangesAsync();
    }

    public async Task<PagedView<View.Stay.Stay>> GetStays(StayGetBinding binding)
    {
        using var context = GetMainContext();
        var query = context.Stays
                           .WhereUser(UserId)
                           .Include(x => x.City)
                           .Include(x => x.Country)
                           .Where(binding);

        return await query.OrderBy(binding)
                    .Select(x => new View.Stay.Stay(x))
                    .ToPagedViewAsync(binding);
    }
}
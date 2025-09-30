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

    public async Task AddStay(StayBinding binding)
    {
        using var context = GetMainContext();
        
        int? cityId = null;
        int countryId;

        if (!string.IsNullOrEmpty(binding.CityId))
        {
            var city = context.Cities.Include(c => c.Country).FirstOrDefault(c => c.ValueId == binding.CityId);
            if (city != null)
            {
                cityId = city.Id;
                countryId = city.CountryId;
            }
            else
                countryId = context.Countries.GetId(binding.CountryId).Value;
        }
        else
            countryId = context.Countries.GetId(binding.CountryId).Value;
        
        var stay = new Database.Travel.Stay()
        {   
            Date = binding.Date,
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
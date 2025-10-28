using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using ProjectIvy.Data.Extensions;
using ProjectIvy.Model.Binding;
using ProjectIvy.Model.Constants.Database;

namespace ProjectIvy.Business.Handlers.WorkDay;

public class WorkDayHandler : Handler<WorkDayHandler>, IWorkDayHandler
{
    public WorkDayHandler(IHandlerContext<WorkDayHandler> context) : base(context)
    {
    }

    public async Task<IEnumerable<Model.View.WorkDay.WorkDay>> Get(FilteredBinding binding)
    {
        using var context = GetMainContext();

        var holidays = await context.Holidays.Where(x => x.Date >= binding.From && x.Date <= binding.To).Select(x => x.Date).ToListAsync();

        var workDays = await context.WorkDays.WhereUser(UserId)
                                             .Where(x => x.Date >= binding.From && x.Date <= binding.To)
                                             .ToListAsync();

        var results = new List<Model.View.WorkDay.WorkDay>();

        for (var date = binding.From.Value; date <= binding.To; date = date.AddDays(1))
        {
            var workDay = workDays.FirstOrDefault(x => x.Date == date);
            results.Add(new Model.View.WorkDay.WorkDay()
            {
                Date = date,
                Type = holidays.Contains(date) ? WorkDayType.Holiday : (workDay != null ? (WorkDayType)workDay.WorkDayTypeId : null)
            });
        }

        return results.OrderByDescending(x => x.Date);
    }
}

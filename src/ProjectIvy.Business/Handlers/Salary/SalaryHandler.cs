using System.Data;
using System.Globalization;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using ProjectIvy.Business.Exceptions;
using ProjectIvy.Business.MapExtensions;
using ProjectIvy.Data.Extensions;
using ProjectIvy.Data.Extensions.Entities;
using ProjectIvy.Model.Binding.Salary;
using ProjectIvy.Model.View;
using View = ProjectIvy.Model.View.Salary;

namespace ProjectIvy.Business.Handlers.Salary;

public class SalaryHandler : Handler<SalaryHandler>, ISalaryHandler
{
    public SalaryHandler(IHandlerContext<SalaryHandler> context) : base(context)
    {
    }

    public async Task<View.Salary> Create(SalaryBinding binding)
    {
        using var context = GetMainContext();
        var entity = await binding.ToEntity(context);
        entity.UserId = UserId;

        using var transaction = await context.Database.BeginTransactionAsync(IsolationLevel.Serializable);
        string prefix = binding.Date.ToString("yyyy-MM-dd-", CultureInfo.InvariantCulture);
        var valueIds = await context.Salaries.WhereUser(UserId)
                                    .Where(x => x.ValueId.StartsWith(prefix))
                                    .Select(x => x.ValueId)
                                    .ToListAsync();
        entity.ValueId = MapExtensions.SalaryExtensions.GetNextValueId(binding.Date, valueIds);

        await context.Salaries.AddAsync(entity);
        await context.SaveChangesAsync();
        await transaction.CommitAsync();
        await context.Entry(entity).Reference(x => x.Currency).LoadAsync();

        return new View.Salary(entity);
    }

    public async Task Delete(string id)
    {
        using var context = GetMainContext();
        var entity = await context.Salaries.WhereUser(UserId)
                                  .SingleOrDefaultAsync(x => x.ValueId == id) ?? throw new ResourceNotFoundException();

        context.Salaries.Remove(entity);
        await context.SaveChangesAsync();
    }

    public async Task<PagedView<View.Salary>> Get(SalaryGetBinding binding)
    {
        using var context = GetMainContext();
        return await context.Salaries.AsNoTracking()
                            .WhereUser(UserId)
                            .Include(x => x.Currency)
                            .Where(binding)
                            .OrderBy(binding.OrderAscending, x => x.Date)
                            .ThenBy(x => x.Id)
                            .Select(x => new View.Salary(x))
                            .ToPagedViewAsync(binding);
    }

    public async Task<View.Salary> GetSingle(string id)
    {
        using var context = GetMainContext();
        var entity = await context.Salaries.AsNoTracking()
                                  .WhereUser(UserId)
                                  .Include(x => x.Currency)
                                  .SingleOrDefaultAsync(x => x.ValueId == id) ?? throw new ResourceNotFoundException();

        return new View.Salary(entity);
    }

    public async Task Update(string id, SalaryBinding binding)
    {
        using var context = GetMainContext();
        var entity = await context.Salaries.WhereUser(UserId)
                                  .SingleOrDefaultAsync(x => x.ValueId == id) ?? throw new ResourceNotFoundException();

        await binding.ToEntity(context, entity);
        await context.SaveChangesAsync();
    }
}

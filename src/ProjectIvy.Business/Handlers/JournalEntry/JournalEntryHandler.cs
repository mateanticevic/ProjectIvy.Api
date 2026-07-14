using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using ProjectIvy.Business.Exceptions;
using ProjectIvy.Data.Extensions;
using ProjectIvy.Model.Binding.JournalEntry;
using ProjectIvy.Model.View;
using Database = ProjectIvy.Model.Database.Main.User;
using View = ProjectIvy.Model.View.JournalEntry;

namespace ProjectIvy.Business.Handlers.JournalEntry;

public class JournalEntryHandler : Handler<JournalEntryHandler>, IJournalEntryHandler
{
    public JournalEntryHandler(IHandlerContext<JournalEntryHandler> context) : base(context)
    {
    }

    public async Task<long> Create(JournalEntryBinding binding)
    {
        using var context = GetMainContext();

        var entity = new Database.JournalEntry
        {
            Date = binding.Date,
            Entry = binding.Entry,
            Created = DateTime.UtcNow,
            Modified = DateTime.UtcNow,
            UserId = UserId
        };

        await context.JournalEntries.AddAsync(entity);
        await context.SaveChangesAsync();

        return entity.Id;
    }

    public async Task Delete(DateOnly date)
    {
        using var context = GetMainContext();

        var entity = await context.JournalEntries.WhereUser(UserId)
                                                 .SingleOrDefaultAsync(x => x.Date == date) ?? throw new ResourceNotFoundException();

        context.JournalEntries.Remove(entity);
        await context.SaveChangesAsync();
    }

    public async Task<PagedView<View.JournalEntry>> Get(JournalEntryGetBinding binding)
    {
        using var context = GetMainContext();

        return await context.JournalEntries.WhereUser(UserId)
                                           .WhereIf(binding.From.HasValue, x => x.Date >= DateOnly.FromDateTime(binding.From.Value))
                                           .WhereIf(binding.To.HasValue, x => x.Date <= DateOnly.FromDateTime(binding.To.Value))
                                           .OrderByDescending(x => x.Date)
                                           .Select(x => new View.JournalEntry(x))
                                           .ToPagedViewAsync(binding);
    }

    public async Task Update(DateOnly date, JournalEntryBinding binding)
    {
        using var context = GetMainContext();

        var entity = await context.JournalEntries.WhereUser(UserId)
                                                 .SingleOrDefaultAsync(x => x.Date == date) ?? throw new ResourceNotFoundException();

        entity.Entry = binding.Entry;
        entity.Modified = DateTime.UtcNow;

        await context.SaveChangesAsync();
    }
}

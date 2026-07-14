using System.Threading.Tasks;
using ProjectIvy.Model.Binding.JournalEntry;
using ProjectIvy.Model.View;
using View = ProjectIvy.Model.View.JournalEntry;

namespace ProjectIvy.Business.Handlers.JournalEntry;

public interface IJournalEntryHandler
{
    Task<long> Create(JournalEntryBinding binding);

    Task Delete(DateOnly date);

    Task<PagedView<View.JournalEntry>> Get(JournalEntryGetBinding binding);

    Task Update(DateOnly date, JournalEntryBinding binding);
}

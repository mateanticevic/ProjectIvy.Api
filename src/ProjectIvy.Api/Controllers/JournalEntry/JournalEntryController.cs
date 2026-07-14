using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using ProjectIvy.Api.Constants;
using ProjectIvy.Business.Handlers.JournalEntry;
using ProjectIvy.Model.Binding.JournalEntry;
using ProjectIvy.Model.View;

namespace ProjectIvy.Api.Controllers.JournalEntry;

[Authorize(ApiScopes.BasicUser)]
[Route("journal/entry")]
public class JournalEntryController : BaseController<JournalEntryController>
{
    private readonly IJournalEntryHandler _journalEntryHandler;

    public JournalEntryController(ILogger<JournalEntryController> logger, IJournalEntryHandler journalEntryHandler) : base(logger)
    {
        _journalEntryHandler = journalEntryHandler;
    }

    [HttpGet]
    public async Task<PagedView<Model.View.JournalEntry.JournalEntry>> Get([FromQuery] JournalEntryGetBinding binding)
        => await _journalEntryHandler.Get(binding);

    [HttpPost]
    public async Task<StatusCodeResult> Post([FromBody] JournalEntryBinding binding)
    {
        await _journalEntryHandler.Create(binding);
        return new StatusCodeResult(StatusCodes.Status201Created);
    }

    [HttpPut("{date}")]
    public async Task Put(DateOnly date, [FromBody] JournalEntryBinding binding)
        => await _journalEntryHandler.Update(date, binding);

    [HttpDelete("{date}")]
    public async Task<StatusCodeResult> Delete(DateOnly date)
    {
        await _journalEntryHandler.Delete(date);
        return new StatusCodeResult(StatusCodes.Status204NoContent);
    }
}

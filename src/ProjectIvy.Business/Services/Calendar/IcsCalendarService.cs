using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using Ical.Net;
using ProjectIvy.Model.View.Calendar;

namespace ProjectIvy.Business.Services.Calendar;

public class IcsCalendarService : IIcsCalendarService
{
    private readonly IHttpClientFactory _httpClientFactory;

    public IcsCalendarService(IHttpClientFactory httpClientFactory)
    {
        _httpClientFactory = httpClientFactory;
    }

    public async Task<IEnumerable<IcsCalendarEvent>> GetEventsAsync(string icsUrl, DateTime from, DateTime to)
    {
        if (string.IsNullOrWhiteSpace(icsUrl))
            return Enumerable.Empty<IcsCalendarEvent>();

        try
        {
            using var httpClient = _httpClientFactory.CreateClient();
            var icsContent = await httpClient.GetStringAsync(icsUrl);

            var calendar = Ical.Net.Calendar.Load(icsContent);

            var events = calendar.Events
                .Where(e => e.Start != null && 
                           e.Start.Value >= from && 
                           e.Start.Value <= to)
                .Select(e => new IcsCalendarEvent
                {
                    Summary = e.Summary,
                    Start = e.Start.Value,
                    End = e.End?.Value,
                    Description = e.Description,
                    Location = e.Location,
                    Uid = e.Uid
                })
                .OrderBy(e => e.Start)
                .ToList();

            return events;
        }
        catch (Exception)
        {
            // Log error if needed
            return Enumerable.Empty<IcsCalendarEvent>();
        }
    }
}

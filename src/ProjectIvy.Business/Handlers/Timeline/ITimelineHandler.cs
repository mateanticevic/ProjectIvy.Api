using ProjectIvy.Model.Binding.Timeline;
using ProjectIvy.Model.View;
using System.Threading.Tasks;
using View = ProjectIvy.Model.View.Timeline;

namespace ProjectIvy.Business.Handlers.Timeline;

public interface ITimelineHandler : IHandler
{
    Task<PagedView<View.TimelineItem>> Get(TimelineGetBinding binding);

    Task<View.TimelineItem> Get(string id);

    Task<int> Count(TimelineGetBinding binding);

    Task<IEnumerable<KeyValuePair<DateTime, int>>> CountByDay(TimelineGetBinding binding);

    Task<IEnumerable<KeyValuePair<int, int>>> CountByYear(TimelineGetBinding binding);
} 
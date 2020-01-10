using ProjectIvy.Model.Binding;
using ProjectIvy.Model.Binding.Tracking;
using ProjectIvy.Model.View;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Xml.Linq;
using View = ProjectIvy.Model.View.Tracking;

namespace ProjectIvy.Business.Handlers.Tracking
{
    public interface ITrackingHandler : IHandler
    {
        bool Create(TrackingBinding binding);

        IEnumerable<View.Tracking> Get(FilteredBinding binding);

        int Count(FilteredBinding binding);

        IEnumerable<GroupedByMonth<int>> CountByMonth(FilteredBinding binding);

        IEnumerable<GroupedByYear<int>> CountByYear(FilteredBinding binding);

        double GetAverageSpeed(FilteredBinding binding);

        Task<IEnumerable<DateTime>> GetDays(TrackingGetBinding binding);

        int GetDistance(FilteredBinding binding);

        double GetMaxSpeed(FilteredBinding binding);

        View.Tracking GetLast(DateTime? at = null);

        int CountUnique(FilteredBinding binding);

        bool ImportFromKml(XDocument kml);
    }
}

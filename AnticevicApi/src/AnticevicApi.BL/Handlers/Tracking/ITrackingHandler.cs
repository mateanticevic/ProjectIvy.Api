using AnticevicApi.Model.Binding.Common;
using AnticevicApi.Model.Binding.Tracking;
using System.Collections.Generic;
using System.Xml.Linq;
using View = AnticevicApi.Model.View.Tracking;

namespace AnticevicApi.BL.Handlers.Tracking
{
    public interface ITrackingHandler : IHandler
    {
        bool Create(TrackingBinding binding);

        IEnumerable<Model.View.Tracking.Tracking> Get(FilteredBinding binding);

        int GetCount(FilteredBinding binding);

        int GetDistance(FilteredBinding binding);

        View.TrackingCurrent GetLast();

        int GetUniqueCount(FilteredBinding binding);

        bool ImportFromKml(XDocument kml);
    }
}

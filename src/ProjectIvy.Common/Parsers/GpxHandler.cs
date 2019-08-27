using ProjectIvy.Common.Interfaces;
using System.Collections.Generic;
using System.Linq;
using System.Xml.Linq;

namespace ProjectIvy.Common.Parsers
{
    public static class GpxHandler
    {
        public static XDocument ToGpx(this IEnumerable<ITracking> trackings)
        {
            var xdoc = new XDocument();

            var root = new XElement("gpx");

            var name = new XElement("name");
            name.Value = $"Tracking";

            root.Add(name);

            var trk = new XElement("trk");

            var trkName = new XElement("name");
            var ordered = trackings.OrderByDescending(x => x.Timestamp);
            trkName.Value = $"{ordered.LastOrDefault().Timestamp} - {ordered.FirstOrDefault().Timestamp}";

            trk.Add(trkName);

            var trkseg = new XElement("trkseg");

            foreach (var tracking in trackings)
            {
                var trkpt = new XElement("trkpt");

                trkpt.Add(new XAttribute("lat", tracking.Latitude));
                trkpt.Add(new XAttribute("lon", tracking.Longitude));

                var time = new XElement("time");
                time.Value = tracking.Timestamp.ToString();

                trkpt.Add(time);

                trkseg.Add(trkpt);
            }

            trk.Add(trkseg);
            root.Add(trk);

            xdoc.Add(root);

            return xdoc;
        }
    }
}

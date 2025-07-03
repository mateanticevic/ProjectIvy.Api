using System.Globalization;
using System.Linq;
using System.Xml.Linq;
using ProjectIvy.Common.Interfaces;
using ProjectIvy.Common.Interfaces.Internal;

namespace ProjectIvy.Common.Parsers;

public static class GpxHandler
{
    public static IEnumerable<ITracking> FromGpx(XDocument xml)
    {
        var elements = xml.Root.Document.Descendants().Single(x => x.Name.LocalName == "trk")
                                 .Descendants().Single(x => x.Name.LocalName == "trkseg")
                                 .Descendants().Where(x => x.Name.LocalName == "trkpt");

        foreach (var trk in elements)
        {
            yield return new Tracking()
            {
                Latitude = decimal.Parse(trk.Attribute("lat").Value, CultureInfo.InvariantCulture),
                Longitude = decimal.Parse(trk.Attribute("lon").Value, CultureInfo.InvariantCulture),
                Timestamp = DateTime.Parse(trk.Descendants().Single(x => x.Name.LocalName == "time").Value)
            };
        }
    }

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

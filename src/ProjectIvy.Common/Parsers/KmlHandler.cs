using ProjectIvy.Common.Interfaces;
using ProjectIvy.Common.Interfaces.Internal;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Xml.Linq;

namespace ProjectIvy.Common.Parsers
{
    public class KmlHandler
    {
        public static IEnumerable<ITracking> ParseKml(XDocument kml)
        {
            XNamespace rootNamespace = kml.Root.Name.Namespace;
            XNamespace namespaceKml = XNamespace.Get("http://www.google.com/kml/ext/2.2");

            var placemark = kml.Root.Element(rootNamespace + "Document")
                                     .Element(rootNamespace + "Placemark");

            var track = placemark.Element(namespaceKml + "Track");

            var timestamps = track.Elements(rootNamespace + "when")
                                  .Select(x => x.Value)
                                  .ToList();
            var coords = track.Elements(namespaceKml + "coord")
                              .Select(x => x.Value.Replace(".", ","))
                              .ToList();

            if (timestamps.Count() != coords.Count())
            {
                // TODO: Replace with appropriate exception
                throw new Exception("Coordinates don't match timestamps.");
            }

            var trackings = new List<Tracking>();

            for (int i = 0; i < timestamps.Count(); i++)
            {
                var location = coords[i].Split(' ');

                var t = new Tracking()
                {
                    Latitude = Math.Round(Convert.ToDecimal(location[1]), 6),
                    Longitude = Math.Round(Convert.ToDecimal(location[0]), 6),
                    Timestamp = DateTime.Parse(timestamps[i])
                };

                trackings.Add(t);
            }

            return trackings;
        }
    }
}

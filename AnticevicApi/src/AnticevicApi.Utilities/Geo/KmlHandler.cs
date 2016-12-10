using AnticevicApi.Model.Database.Main.Tracking;
using System.Collections.Generic;
using System.Linq;
using System.Xml.Linq;
using System;

namespace AnticevicApi.Utilities.Geo
{
    public class KmlHandler
    {
        public static IEnumerable<Tracking> ParseKml(XDocument kml)
        {
            XNamespace ns = kml.Root.Name.Namespace;
            XNamespace nsGx = XNamespace.Get("http://www.google.com/kml/ext/2.2");

            var placemark = kml.Root.Element(ns + "Document")
                                     .Element(ns + "Placemark");

            var track = placemark.Element(nsGx + "Track");

            var timestamps = track.Elements(ns + "when")
                                  .Select(x => x.Value)
                                  .ToList();
            var coords = track.Elements(nsGx + "coord")
                              .Select(x => x.Value.Replace(".", ","))
                              .ToList();

            if(timestamps.Count() != coords.Count())
            {
                //TODO: Replace with appropriate exception
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

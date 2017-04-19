using AnticevicApi.BL.MapExtensions;
using AnticevicApi.DL.Extensions;
using AnticevicApi.Model.Binding.Common;
using AnticevicApi.Model.Binding.Tracking;
using AnticevicApi.Utilities.Geo;
using GeoCoordinatePortable;
using System.Collections.Generic;
using System.Linq;
using System.Xml.Linq;
using System;
using View = AnticevicApi.Model.View.Tracking;

namespace AnticevicApi.BL.Handlers.Tracking
{
    public class TrackingHandler : Handler<TrackingHandler>, ITrackingHandler
    {
        public TrackingHandler(IHandlerContext<TrackingHandler> context) : base(context)
        {
        }

        public bool Create(TrackingBinding binding)
        {
            using (var db = GetMainContext())
            {
                var tracking = binding.ToEntity();
                tracking.User.Id = User.Id;

                db.Trackings.Add(tracking);
                db.SaveChanges();

                return true;
            }
        }

        public IEnumerable<View.Tracking> Get(FilteredBinding binding)
        {
            using (var db = GetMainContext())
            {
                return db.Trackings.WhereUser(User.Id)
                                   .WhereTimestampInclusive(binding)
                                   .OrderBy(x => x.Timestamp)
                                   .ToList()
                                   .Select(x => new View.Tracking(x));
            }
        }

        public int GetCount(FilteredBinding binding)
        {
            using (var db = GetMainContext())
            {
                var userTrackings = db.Trackings.WhereUser(User.Id)
                                                .WhereTimestampInclusive(binding);

                return userTrackings.Count();
            }
        }

        public int GetDistance(FilteredBinding binding)
        {
            using (var db = GetMainContext())
            {
                int total = db.TrackingDistances.WhereUser(User.Id)
                                                .WhereTimestampInclusive(binding)
                                                .Sum(x => x.DistanceInMeters);

                var lastDate = db.TrackingDistances.WhereUser(User.Id)
                                                   .OrderByDescending(x => x.Timestamp)
                                                   .FirstOrDefault()
                                                   .Timestamp;

                binding.To = binding.To.HasValue ? binding.To : DateTime.Now;

                if (lastDate < binding.To.Value.Date)
                {
                    var from = lastDate.AddDays(1) < binding.From ? binding.From : lastDate.AddDays(1);

                    // TODO: Include last tracking from previous date
                    var trackings = db.Trackings.WhereUser(User.Id)
                                                .Where(x => x.Timestamp > from && x.Timestamp < binding.To.Value)
                                                .OrderBy(x => x.Timestamp)
                                                .ToList()
                                                .Select(x => new GeoCoordinate((double)x.Latitude, (double)x.Longitude, (double)x.Altitude))
                                                .ToList();
                    double sum = 0;
                    for (int i = 0; i < trackings.Count() - 1; i++)
                    {
                        sum += trackings[i].GetDistanceTo(trackings[i + 1]);
                    }

                    total += (int)sum;
                }

                return total;
            }
        }

        public View.TrackingCurrent GetLast()
        {
            using (var db = GetMainContext())
            {
                var tracking = db.Trackings.WhereUser(User.Id)
                                           .OrderByDescending(x => x.Timestamp)
                                           .FirstOrDefault();

                return new View.TrackingCurrent(tracking);
            }
        }

        public int GetUniqueCount(FilteredBinding binding)
        {
            using (var db = GetMainContext())
            {
                var query = db.UniqueLocations.WhereUser(User.Id)
                                              .WhereTimestampInclusive(binding);

                return query.Count();
            }
        }

        public bool ImportFromKml(XDocument kml)
        {
            var trackings = KmlHandler.ParseKml(kml);

            using (var db = GetMainContext())
            {
                foreach (var t in trackings)
                {
                    t.User.Id = User.Id;
                }

                db.Trackings.AddRange(trackings);
                db.SaveChanges();

                return true;
            }
        }
    }
}

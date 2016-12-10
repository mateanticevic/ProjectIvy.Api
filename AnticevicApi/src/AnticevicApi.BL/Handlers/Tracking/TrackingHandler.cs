using AnticevicApi.BL.MapExtensions;
using AnticevicApi.DL.DbContexts;
using AnticevicApi.DL.Extensions;
using AnticevicApi.Model.Binding.Common;
using AnticevicApi.Model.Binding.Tracking;
using AnticevicApi.Utilities.Geo;
using GeoCoordinatePortable;
using System.Collections.Generic;
using System.Linq;
using System.Xml.Linq;
using View = AnticevicApi.Model.View.Tracking;

namespace AnticevicApi.BL.Handlers.Tracking
{
    public class TrackingHandler : Handler, ITrackingHandler
    {
        public bool Create(TrackingBinding binding)
        {
            using (var db = new MainContext(ConnectionString))
            {
                var tracking = binding.ToEntity();
                tracking.UserId = UserId;

                db.Trackings.Add(tracking);
                db.SaveChanges();

                return true;
            }
        }

        public IEnumerable<View.Tracking> Get(FilteredBinding binding)
        {
            using (var db = new MainContext(ConnectionString))
            {
                return db.Trackings.WhereUser(UserId)
                                   .WhereTimestampInclusive(binding)
                                   .OrderBy(x => x.Timestamp)
                                   .ToList()
                                   .Select(x => new View.Tracking(x));
            }
        }

        public int GetCount(FilteredBinding binding)
        {
            using (var db = new MainContext(ConnectionString))
            {
                var userTrackings = db.Trackings.WhereUser(UserId)
                                                .WhereTimestampInclusive(binding);

                return userTrackings.Count();
            }
        }

        public int GetDistance(FilteredBinding binding)
        {
            using (var db = new MainContext(ConnectionString))
            {
                int total = db.TrackingDistances.WhereUser(UserId)
                                                .WhereTimestampInclusive(binding)
                                                .Sum(x => x.DistanceInMeters);

                var lastDate = db.TrackingDistances.WhereUser(UserId)
                                                   .OrderByDescending(x => x.Timestamp)
                                                   .FirstOrDefault()
                                                   .Timestamp;

                if(lastDate < binding.To.Value.Date)
                {
                    var from = lastDate.AddDays(1) < binding.From ? binding.From : lastDate.AddDays(1);

                    //TODO: Include last tracking from previous date
                    var trackings = db.Trackings.WhereUser(UserId)
                                                .Where(x => x.Timestamp > from && x.Timestamp < binding.To.Value.AddDays(1))
                                                .OrderBy(x => x.Timestamp)
                                                .ToList()
                                                .Select(x => new GeoCoordinate((double)x.Latitude, (double)x.Longitude, (double)x.Altitude))
                                                .ToList();
                    double sum = 0;
                    for (int i = 0; i < trackings.Count() - 1; i++)
                    {
                        sum += trackings[i].GetDistanceTo(trackings[i+1]);
                    }

                    total += (int)sum;
                }

                return total;
            }
        }

        public View.TrackingCurrent GetLast()
        {
            using (var db = new MainContext(ConnectionString))
            {
                var tracking = db.Trackings.WhereUser(UserId)
                                           .OrderByDescending(x => x.Timestamp)
                                           .FirstOrDefault();

                return new View.TrackingCurrent(tracking);
            }
        }

        public int GetUniqueCount(FilteredBinding binding)
        {
            using (var db = new MainContext(ConnectionString))
            {
                var query = db.UniqueLocations.WhereUser(UserId)
                                              .WhereTimestampInclusive(binding);

                return query.Count();
            }
        }

        public bool ImportFromKml(XDocument kml)
        {
            var trackings = KmlHandler.ParseKml(kml);

            using (var db = new MainContext(ConnectionString))
            {
                foreach (var t in trackings)
                {
                    t.UserId = UserId;
                }

                db.Trackings.AddRange(trackings);
                db.SaveChanges();

                return true;
            }
        }
    }
}

using ProjectIvy.BL.MapExtensions;
using ProjectIvy.DL.Extensions.Entities;
using ProjectIvy.DL.Extensions;
using ProjectIvy.Model.Binding.Common;
using ProjectIvy.Model.Binding.Tracking;
using ProjectIvy.Utilities.Geo;
using System.Collections.Generic;
using System.Linq;
using System.Xml.Linq;
using System;
using View = ProjectIvy.Model.View.Tracking;

namespace ProjectIvy.BL.Handlers.Tracking
{
    public class TrackingHandler : Handler<TrackingHandler>, ITrackingHandler
    {
        public TrackingHandler(IHandlerContext<TrackingHandler> context) : base(context)
        {
        }

        public int Count(FilteredBinding binding)
        {
            using (var db = GetMainContext())
            {
                var userTrackings = db.Trackings.WhereUser(User.Id)
                                                .WhereTimestampInclusive(binding);

                return userTrackings.Count();
            }
        }

        public int CountUnique(FilteredBinding binding)
        {
            using (var db = GetMainContext())
            {
                var query = db.UniqueLocations.WhereUser(User.Id)
                                              .WhereTimestampInclusive(binding);

                return query.Count();
            }
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

        public int GetDistance(FilteredBinding binding)
        {
            binding.From = binding.From.HasValue ? binding.From : DateTime.MinValue;
            binding.To = binding.To.HasValue ? binding.To : DateTime.MaxValue;

            using (var db = GetMainContext())
            {
                int total = 0;

                // Already calculated distance. By day.
                {
                    var from = binding.From.Value.TimeOfDay != TimeSpan.Zero ? binding.From.Value.Date.AddDays(1) : binding.From.Value;
                    var to = binding.To.Value.Date;

                    total = db.TrackingDistances.WhereUser(User.Id)
                                                    .WhereTimestampFromInclusive(from, to)
                                                    .Sum(x => x.DistanceInMeters);
                }

                var lastDate = db.TrackingDistances.WhereUser(User.Id)
                                                   .OrderByDescending(x => x.Timestamp)
                                                   .FirstOrDefault()
                                                   .Timestamp;

                if (lastDate > binding.From.Value && binding.From.Value.TimeOfDay != TimeSpan.Zero)
                {
                    var to = binding.From.Value.Date == binding.To.Value.Date ? binding.To.Value : binding.From.Value.Date.AddDays(1);

                    total += db.Trackings.WhereUser(User.Id)
                                         .Distance(binding.From.Value, to);
                }

                if (lastDate > binding.To.Value && binding.To.Value.TimeOfDay != TimeSpan.Zero && binding.From.Value.Date != binding.To.Value.Date)
                {
                    total += db.Trackings.WhereUser(User.Id)
                                         .Distance(binding.To.Value.Date, binding.To.Value);
                }

                binding.To = binding.To.HasValue ? binding.To : DateTime.Now;

                if (lastDate < binding.To.Value.Date)
                {
                    var from = lastDate.AddDays(1) < binding.From ? binding.From : lastDate.AddDays(1);

                    // TODO: Include last tracking from previous date
                    total += db.Trackings.WhereUser(User.Id)
                                         .Distance(from.Value, binding.To.Value);
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

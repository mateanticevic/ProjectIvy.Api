using ProjectIvy.BL.MapExtensions;
using ProjectIvy.Common.Parsers;
using ProjectIvy.Data.Extensions.Entities;
using ProjectIvy.Data.Extensions;
using ProjectIvy.Model.Binding.Tracking;
using System.Collections.Generic;
using System.Linq;
using System.Xml.Linq;
using System;
using ProjectIvy.Model.Binding;
using ProjectIvy.Model.View;
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

        public IEnumerable<GroupedByMonth<int>> CountByMonth(FilteredBinding binding)
        {
            using (var context = GetMainContext())
            {
                return context.Trackings.WhereUser(User)
                              .WhereTimestampInclusive(binding.From, binding.To)
                              .GroupBy(x => new { x.Timestamp.Year, x.Timestamp.Month })
                              .OrderByDescending(x => x.Key.Year)
                              .ThenByDescending(x => x.Key.Month)
                              .Select(x => new GroupedByMonth<int>(x.Count(), x.Key.Year, x.Key.Month))
                              .ToList();
            }
        }

        public IEnumerable<GroupedByYear<int>> CountByYear(FilteredBinding binding)
        {
            using (var context = GetMainContext())
            {
                return context.Trackings.WhereUser(User)
                                        .WhereTimestampInclusive(binding.From, binding.To)
                                        .GroupBy(x => x.Timestamp.Year)
                                        .OrderByDescending(x => x.Key)
                                        .Select(x => new GroupedByYear<int>(x.Count(), x.Key))
                                        .ToList();
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
            binding.From = binding.From ?? DateTime.MinValue;
            binding.To = binding.To ?? DateTime.MaxValue;

            using (var db = GetMainContext())
            {
                var total = 0;

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

        public double GetAverageSpeed(FilteredBinding binding)
        {
            using (var context = GetMainContext())
            {
                var averageSpeed = context.Trackings.WhereUser(User)
                                                    .WhereTimestampInclusive(binding)
                                                    .Average(x => x.Speed);

                return averageSpeed ?? 0;
            }
        }

        public View.Tracking GetLast(DateTime? at = null)
        {
            using (var db = GetMainContext())
            {
                var tracking = db.Trackings.WhereUser(User.Id)
                                           .WhereIf(at.HasValue, x => x.Timestamp < at.Value)
                                           .OrderByDescending(x => x.Timestamp)
                                           .FirstOrDefault();

                return new View.Tracking(tracking);
            }
        }

        public double GetMaxSpeed(FilteredBinding binding)
        {
            using (var context = GetMainContext())
            {
                var maxSpeed = context.Trackings.WhereUser(User)
                                                .WhereTimestampInclusive(binding)
                                                .Max(x => x.Speed);

                return maxSpeed ?? 0;
            }
        }

        public bool ImportFromKml(XDocument kml)
        {
            var trackings = KmlHandler.ParseKml(kml)
                                      .Select(x => (Model.Database.Main.Tracking.Tracking)x);

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

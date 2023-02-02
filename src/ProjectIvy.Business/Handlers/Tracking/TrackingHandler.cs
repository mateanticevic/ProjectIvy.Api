using GeoCoordinatePortable;
using Geohash;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using ProjectIvy.Business.MapExtensions;
using ProjectIvy.Common.Parsers;
using ProjectIvy.Data.Extensions;
using ProjectIvy.Data.Extensions.Entities;
using ProjectIvy.Model.Binding;
using ProjectIvy.Model.Binding.Tracking;
using ProjectIvy.Model.Services.LastFm;
using ProjectIvy.Model.View;
using System.Linq;
using System.Threading.Tasks;
using System.Xml.Linq;
using View = ProjectIvy.Model.View.Tracking;

namespace ProjectIvy.Business.Handlers.Tracking
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
                var userTrackings = db.Trackings.WhereUser(UserId)
                                                .WhereTimestampInclusive(binding);

                return userTrackings.Count();
            }
        }

        public IEnumerable<GroupedByMonth<int>> CountByMonth(FilteredBinding binding)
        {
            using (var context = GetMainContext())
            {
                return context.Trackings.WhereUser(UserId)
                              .WhereTimestampInclusive(binding.From, binding.To)
                              .GroupBy(x => new { x.Timestamp.Year, x.Timestamp.Month })
                              .OrderByDescending(x => x.Key.Year)
                              .ThenByDescending(x => x.Key.Month)
                              .Select(x => new GroupedByMonth<int>(x.Count(), x.Key.Year, x.Key.Month))
                              .ToList();
            }
        }

        public IEnumerable<KeyValuePair<int, int>> CountByYear(FilteredBinding binding)
        {
            using (var context = GetMainContext())
            {
                return context.Trackings.WhereUser(UserId)
                                        .WhereTimestampInclusive(binding.From, binding.To)
                                        .GroupBy(x => x.Timestamp.Year)
                                        .OrderByDescending(x => x.Key)
                                        .Select(x => new KeyValuePair<int, int>(x.Count(), x.Key))
                                        .ToList();
            }
        }

        public int CountUnique(FilteredBinding binding)
        {
            throw new NotImplementedException();
        }

        public bool Create(TrackingBinding binding)
        {
            using (var db = GetMainContext())
            {
                var geohasher = new Geohasher();

                var tracking = binding.ToEntity();
                tracking.Geohash = geohasher.Encode((double)binding.Latitude, (double)binding.Longitude, 9);
                tracking.UserId = UserId;

                db.Trackings.Add(tracking);
                db.SaveChanges();

                return true;
            }
        }

        public async Task Create(IEnumerable<TrackingBinding> binding)
        {
            try
            {
                var geohasher = new Geohasher();

                using (var db = GetMainContext())
                {
                    foreach (var b in binding)
                    {
                        b.Timestamp = new DateTime(b.Timestamp.Year, b.Timestamp.Month, b.Timestamp.Day, b.Timestamp.Hour, b.Timestamp.Minute, b.Timestamp.Second, b.Timestamp.Millisecond);
                    }

                    var timestamps = binding.Select(x => x.Timestamp).ToList();
                    Logger.LogInformation("Trying to save {TrackingCount} trackings", timestamps.Count);

                    var existingTimestamps = await db.Trackings.WhereUser(UserId)
                                                               .Where(x => timestamps.Contains(x.Timestamp))
                                                               .Select(x => x.Timestamp)
                                                               .ToListAsync();

                    Logger.LogInformation("Found {TrackingCount} duplicate trackings", existingTimestamps.Count);

                    var trackings = binding.GroupBy(x => x.Timestamp)
                                           .Select(x => x.First())
                                           .Where(x => !existingTimestamps.Contains(x.Timestamp))
                                           .Select(x =>
                    {
                        var entity = x.ToEntity();
                        entity.Geohash = geohasher.Encode((double)x.Latitude, (double)x.Longitude, 9);
                        entity.UserId = UserId;
                        return entity;
                    }).ToList();
                    await db.Trackings.AddRangeAsync(trackings);

                    int affeted = await db.SaveChangesAsync();
                    Logger.LogInformation("Inserted {TrackingCount} trackins", affeted);
                }
            }
            catch (Exception e)
            {
                Logger.LogError(e?.InnerException, "Failed to save trackings");
            }
        }

        public async Task Delete(IEnumerable<long> timestamps)
        {
            var dateTimes = timestamps.Select(x => DateTimeOffset.FromUnixTimeMilliseconds(x).UtcDateTime);
            using (var context = GetMainContext())
            {
                var trackings = await context.Trackings.WhereUser(UserId)
                                                       .Where(x => dateTimes.Contains(x.Timestamp))
                                                       .ToListAsync();

                context.Trackings.RemoveRange(trackings);
                Logger.LogInformation("Removed {TrackingCount} trackings", trackings.Count);

                await context.SaveChangesAsync();
            }
        }

        public IEnumerable<View.Tracking> Get(TrackingGetBinding binding)
        {
            using (var db = GetMainContext())
            {
                return db.Trackings.WhereUser(UserId)
                                   .WhereTimestampInclusive(binding)
                                   .WhereIf(binding.BottomRight != null && binding.TopLeft != null, x => x.Longitude > binding.TopLeft.Lng && x.Longitude < binding.BottomRight.Lng && x.Latitude < binding.TopLeft.Lat && x.Latitude > binding.BottomRight.Lat)
                                   .OrderBy(x => x.Timestamp)
                                   .ToList()
                                   .Select(x => new View.Tracking(x));
            }
        }

        public async Task<IEnumerable<string>> GetDays(TrackingGetBinding binding)
        {
            using (var context = GetMainContext())
            {
                return (await context.Trackings.WhereUser(UserId)
                                              .WhereIf(binding.BottomRight != null && binding.TopLeft != null, x => x.Longitude > binding.TopLeft.Lng && x.Longitude < binding.BottomRight.Lng && x.Latitude < binding.TopLeft.Lat && x.Latitude > binding.BottomRight.Lat)
                                              .Select(x => x.Timestamp.Date)
                                              .Distinct()
                                              .OrderByDescending(x => x)
                                              .ToListAsync()).Select(x => x.ToString("yyyy-MM-dd"));
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

                    total = db.TrackingDistances.WhereUser(UserId)
                                                    .WhereTimestampFromInclusive(from, to)
                                                    .Sum(x => x.DistanceInMeters);
                }

                var lastDate = db.TrackingDistances.WhereUser(UserId)
                                                   .OrderByDescending(x => x.Timestamp)
                                                   .FirstOrDefault()
                                                   .Timestamp;

                if (lastDate > binding.From.Value && binding.From.Value.TimeOfDay != TimeSpan.Zero)
                {
                    var to = binding.From.Value.Date == binding.To.Value.Date ? binding.To.Value : binding.From.Value.Date.AddDays(1);

                    total += db.Trackings.WhereUser(UserId)
                                         .Distance(binding.From.Value, to);
                }

                if (lastDate > binding.To.Value && binding.To.Value.TimeOfDay != TimeSpan.Zero && binding.From.Value.Date != binding.To.Value.Date)
                {
                    total += db.Trackings.WhereUser(UserId)
                                         .Distance(binding.To.Value.Date, binding.To.Value);
                }

                binding.To = binding.To.HasValue ? binding.To : DateTime.Now;

                if (lastDate < binding.To.Value.Date)
                {
                    var from = lastDate.AddDays(1) < binding.From ? binding.From : lastDate.AddDays(1);

                    // TODO: Include last tracking from previous date
                    total += db.Trackings.WhereUser(UserId)
                                         .Distance(from.Value, binding.To.Value);
                }

                return total;
            }
        }

        public double GetAverageSpeed(FilteredBinding binding)
        {
            using (var context = GetMainContext())
            {
                var averageSpeed = context.Trackings.WhereUser(UserId)
                                                    .WhereTimestampInclusive(binding)
                                                    .Average(x => x.Speed);

                return averageSpeed ?? 0;
            }
        }

        public async Task<View.Tracking> GetLast(DateTime? at = null) => new View.Tracking(await GetLastTracking(at));

        public async Task<View.TrackingLocation> GetLastLocation()
        {
            var tracking = await GetLastTracking();
            var trackingCoordiante = new GeoCoordinate((double)tracking.Latitude, (double)tracking.Longitude, tracking.Altitude ?? 0);

            using (var context = GetMainContext())
            {
                var userLocations = await context.Locations.WhereUser(UserId)
                                                           .Include(x => x.LocationType)
                                                           .ToListAsync();
                var location = userLocations.FirstOrDefault(x => trackingCoordiante.GetDistanceTo(x.ToGeoCoordinate()) < x.Radius);
                return new View.TrackingLocation()
                {
                    Country = await GeohashToCountry(tracking.Geohash),
                    Location = location != null ? new View.Location(location) : null,
                    Tracking = new View.Tracking(tracking)
                };
            }
        }

        public double GetMaxSpeed(FilteredBinding binding)
        {
            using (var context = GetMainContext())
            {
                var maxSpeed = context.Trackings.WhereUser(UserId)
                                                .WhereTimestampInclusive(binding)
                                                .Max(x => x.Speed);

                return maxSpeed ?? 0;
            }
        }

        public async Task ImportFromGpx(XDocument xml)
        {
            var trackings = GpxHandler.FromGpx(xml);
            var geohasher = new Geohasher();

            using (var db = GetMainContext())
            {
                var entities = trackings.Select(x => new Model.Database.Main.Tracking.Tracking()
                {
                    Geohash = geohasher.Encode((double)x.Latitude, (double)x.Longitude, 9),
                    Longitude = x.Longitude,
                    Latitude = x.Latitude,
                    Timestamp = x.Timestamp,
                    UserId = UserId
                }).ToList();

                await db.Trackings.AddRangeAsync(entities);
                await db.SaveChangesAsync();
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
                    t.UserId = UserId;
                }

                db.Trackings.AddRange(trackings);
                db.SaveChanges();

                return true;
            }
        }

        private async Task<Model.View.Country.Country> GeohashToCountry(string geohash)
        {
            using (var context = GetMainContext())
            {
                var geohashes = Enumerable.Range(0, geohash.Length - 2)
                                          .Select(x => geohash.Substring(0, geohash.Length - x))
                                          .ToList();

                return await context.GeohashCountries.Include(x => x.Country)
                                                     .Where(x => geohashes.Contains(x.Geohash))
                                                     .OrderByDescending(x => x.Geohash.Length)
                                                     .Select(x => new Model.View.Country.Country(x.Country))
                                                     .FirstOrDefaultAsync();
            }
        }

        public async Task<Model.Database.Main.Tracking.Tracking> GetLastTracking(DateTime? at = null)
        {
            using (var db = GetMainContext())
            {
                return await db.Trackings.WhereUser(UserId)
                                         .WhereIf(at.HasValue, x => x.Timestamp < at.Value)
                                         .OrderByDescending(x => x.Timestamp)
                                         .FirstOrDefaultAsync();
            }
        }
    }
}

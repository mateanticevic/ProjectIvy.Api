using System.Linq;
using System.Threading.Tasks;
using System.Xml.Linq;
using GeoCoordinatePortable;
using Geohash;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Logging;
using ProjectIvy.Business.Caching;
using ProjectIvy.Business.Handlers.Geohash;
using ProjectIvy.Business.MapExtensions;
using ProjectIvy.Common.Parsers;
using ProjectIvy.Data.Extensions;
using ProjectIvy.Data.Extensions.Entities;
using ProjectIvy.Model.Binding;
using ProjectIvy.Model.Binding.Tracking;
using ProjectIvy.Model.View;
using ProjectIvy.Model.View.Tracking;
using View = ProjectIvy.Model.View.Tracking;

namespace ProjectIvy.Business.Handlers.Tracking
{
    public class TrackingHandler : Handler<TrackingHandler>, ITrackingHandler
    {
        private readonly IGeohashHandler _geohashHandler;

        public TrackingHandler(IHandlerContext<TrackingHandler> context,
                               IGeohashHandler geohashHandler,
                               IMemoryCache memoryCache) : base(context, memoryCache, nameof(TrackingHandler))
        {
            _geohashHandler = geohashHandler;
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

        public async Task Create(TrackingBinding binding)
        {
            using (var db = GetMainContext())
            {
                var geohasher = new Geohasher();

                var tracking = binding.ToEntity();
                tracking.Geohash = geohasher.Encode((double)binding.Latitude, (double)binding.Longitude, 9);
                tracking.UserId = UserId;

                await db.Trackings.AddAsync(tracking);
                await db.SaveChangesAsync();
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
                                           .Select(x => x.First())
                                           .Where(x => !existingTimestamps.Contains(x.Timestamp))
                                           .Select(x =>
                    {
                        var entity = x.ToEntity();
                        entity.Geohash = geohasher.Encode((double)x.Latitude, (double)x.Longitude, 9);
                        entity.UserId = UserId;
                        return entity;
                    }).ToList();
                    await db.Trackings.AddRangeAsync(trackings);

                    int count = await db.SaveChangesAsync();
                    Logger.LogInformation("Inserted {TrackingCount} trackings", count);
                }
            }
            catch (Exception e)
            {
                Logger.LogError(e, "Failed to save trackings");
                throw;
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
                                   .WhereIf(binding.BottomRight != null && binding.TopLeft != null, x => x.Longitude > binding.TopLeft.Longitude && x.Longitude < binding.BottomRight.Longitude && x.Latitude < binding.TopLeft.Latitude && x.Latitude > binding.BottomRight.Latitude)
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
                                              .WhereIf(binding.BottomRight != null && binding.TopLeft != null, x => x.Longitude > binding.TopLeft.Longitude && x.Longitude < binding.BottomRight.Longitude && x.Latitude < binding.TopLeft.Latitude && x.Latitude > binding.BottomRight.Latitude)
                                              .Select(x => x.Timestamp.Date)
                                              .Distinct()
                                              .OrderByDescending(x => x)
                                              .ToListAsync()).Select(x => x.ToString("yyyy-MM-dd"));
            }
        }

        public int GetDistance(FilteredBinding binding)
        {
            string cacheKey = BuildUserCacheKey(CacheKeyGenerator.TrackingsGetDistance(binding.From, binding.To));

            return MemoryCache.GetOrCreate(cacheKey, cacheEntry =>
            {
                cacheEntry.AbsoluteExpirationRelativeToNow = TimeSpan.FromMinutes(1);
                return GetDistanceNonCached(binding);
            });
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

        public async Task<TrackingDetails> GetDetails(FilteredBinding binding)
        {
            using var context = GetMainContext();

            var trackings = await context.Trackings.WhereUser(UserId)
                                                   .WhereTimestampInclusive(binding)
                                                   .OrderBy(x => x.Timestamp)
                                                   .ToListAsync();

            double ascentInMeters = 0;
            double descentInMeters = 0;
            double? previousAltitude = null;
            foreach (var tracking in trackings)
            {
                if (previousAltitude.HasValue && tracking.Altitude.HasValue && tracking.Altitude > previousAltitude)
                    ascentInMeters += tracking.Altitude.Value - previousAltitude.Value;

                if (previousAltitude.HasValue && tracking.Altitude.HasValue && tracking.Altitude < previousAltitude)
                    descentInMeters += previousAltitude.Value - tracking.Altitude.Value;

                previousAltitude = tracking.Altitude;
            }

            return new TrackingDetails
            {
                AscentInMeters = (int)ascentInMeters,
                DescentInMeters = (int)descentInMeters,
                ElevationGainInMeters = (int)trackings.Max(x => x.Altitude) - (int)trackings.Min(x => x.Altitude)
            };
        }

        public async Task<View.Tracking> GetLast(DateTime? at = null) => new View.Tracking(await GetLastTracking(at));

        public async Task<IEnumerable<DateTime>> GetDaysAtLast(DateTime? at = null)
        {
            using var context = GetMainContext();

            var last = await context.Trackings.WhereUser(UserId)
                                              .WhereIf(at.HasValue, x => x.Timestamp < at.Value)
                                              .OrderByDescending(x => x.Timestamp)
                                              .FirstOrDefaultAsync();

            string parentGeohash = last.Geohash.Substring(0, 7);

            return await context.Trackings.WhereUser(UserId)
                                          .WhereIf(at.HasValue, x => x.Timestamp < at.Value)
                                          .Where(x => x.Timestamp.Date != (at.HasValue ? at.Value.Date : DateTime.Now.Date))
                                          .Where(x => x.Geohash.StartsWith(parentGeohash))
                                          .Select(x => x.Timestamp.Date)
                                          .Distinct()
                                          .OrderByDescending(x => x)
                                          .ToListAsync();
        }

        public async Task<TrackingLocation> GetLastLocation()
        {
            var tracking = await GetLastTracking();
            var trackingCoordiante = new GeoCoordinate((double)tracking.Latitude, (double)tracking.Longitude, tracking.Altitude ?? 0);

            var locationGeohashes = await MemoryCache.GetOrCreateAsync(BuildUserCacheKey(CacheKeyGenerator.LocationGeohashes()), async cacheKey =>
            {
                cacheKey.AbsoluteExpirationRelativeToNow = TimeSpan.FromHours(1);
                using var db = GetMainContext();

                return await db.Locations.WhereUser(UserId)
                                         .Include(x => x.Geohashes)
                                         .Include(x => x.LocationType)
                                         .ToListAsync();
            });

            var location = locationGeohashes.Where(x => x.Geohashes.Any(y => tracking.Geohash.StartsWith(y.Geohash)))
                                            .OrderBy(x => x.CanContainOtherLocations)
                                            .FirstOrDefault();

            using var context = GetMainContext();
            var flight = await context.Flights.WhereUser(UserId)
                                              .Include(x => x.Airline)
                                              .Include(x => x.DestinationAirport)
                                              .ThenInclude(x => x.Poi)
                                              .Include(x => x.OriginAirport)
                                              .ThenInclude(x => x.Poi)
                                              .Where(x => x.DateOfDeparture.AddHours(-2) < DateTime.UtcNow && x.DateOfArrival.AddHours(6) > DateTime.UtcNow)
                                              .FirstOrDefaultAsync();

            var trackingLocation = new View.TrackingLocation
            {
                Country = await _geohashHandler.GetCountry(tracking.Geohash) ?? await _geohashHandler.GetCountry(tracking.Geohash),
                Flight = flight is not null ? new Model.View.Flight.Flight(flight) : null,
                Location = location is not null ? new View.KnownLocation(location) : null,
                Tracking = new View.Tracking(tracking),
                City = await _geohashHandler.GetCity(tracking.Geohash)
            };

            return trackingLocation;
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

        private int GetDistanceNonCached(FilteredBinding binding)
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
    }
}

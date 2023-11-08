using System.Diagnostics.Metrics;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Hosting;

namespace ProjectIvy.Api.Services
{
    public class MetricsBackgroundService : BackgroundService
    {
        private readonly IMemoryCache _memoryCache;
        private readonly Meter _meter;

		public MetricsBackgroundService(IMemoryCache memoryCache)
		{
            _memoryCache = memoryCache;
            _meter = new Meter("memory_cache");
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            while (!stoppingToken.IsCancellationRequested)
            {
                var stats = _memoryCache.GetCurrentStatistics();

                if (stats is not null)
                {
                    _meter.CreateObservableGauge("item_count", () => stats.CurrentEntryCount);
                    _meter.CreateObservableGauge("size", () => stats.CurrentEstimatedSize ?? 0);
                    _meter.CreateObservableGauge("hits", () => stats.TotalHits);
                    _meter.CreateObservableGauge("misses", () => stats.TotalMisses);
                }

                await Task.Delay(TimeSpan.FromSeconds(10), stoppingToken);
            }
        }
    }
}


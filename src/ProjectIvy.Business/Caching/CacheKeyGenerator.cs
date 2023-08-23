namespace ProjectIvy.Business.Caching
{
    public static class CacheKeyGenerator
	{
		public static string TrackingGetDistance(DateTime? from, DateTime? to) => $"Tracking_GetDistance_{from}_{to}";
	}
}


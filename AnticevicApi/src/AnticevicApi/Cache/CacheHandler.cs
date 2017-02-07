using Microsoft.Extensions.Caching.Memory;

namespace AnticevicApi.Cache
{
    public class CacheHandler
    {
        private static CacheHandler _instance;

        public static CacheHandler Instance
        {
            get
            {
                if (_instance == null)
                {
                    _instance = new CacheHandler();
                    var options = new MemoryCacheOptions();
                    _instance.Cache = new MemoryCache(options);
                }

                return _instance;
            }
        }

        public MemoryCache Cache { get; set; }
    }
}

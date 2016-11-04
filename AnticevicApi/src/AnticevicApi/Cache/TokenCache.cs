using AnticevicApi.Model.Database.Main.Security;
using Microsoft.Extensions.Caching.Memory;

namespace AnticevicApi.Cache
{
    public class TokenCache
    {
        public static AccessToken Get(string token)
        {
            return CacheHandler.Instance.Cache.Get<AccessToken>($"Token.{token}");
        }

        public static void Set(AccessToken token)
        {
            CacheHandler.Instance.Cache.Set($"Token.{token.Token}", token);
        }
    }
}

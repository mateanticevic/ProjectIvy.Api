using Microsoft.Extensions.Caching.Memory;
using ProjectIvy.Model.Database.Main.Security;
using ProjectIvy.Model.Database.Main.User;
using System;

namespace ProjectIvy.Api.Cache
{
    public class TokenCache
    {
        public static AccessToken Get(string token)
        {
            return CacheHandler.Instance.Cache.Get<AccessToken>($"Token.{token}");
        }

        public static void Set(AccessToken token)
        {
            CacheHandler.Instance.Cache.Set($"Token.{token.Token}", token, new MemoryCacheEntryOptions() { AbsoluteExpiration = DateTime.Now.AddMinutes(10) });
        }

        public static User GetUser(string token)
        {
            return CacheHandler.Instance.Cache.Get<User>($"User.{token}");
        }

        public static void SetUser(User user, string token)
        {
            CacheHandler.Instance.Cache.Set($"User.{token}", user);
        }
    }
}

using AnticevicApi.DL.DbContexts;
using AnticevicApi.Model.Database.Main.Security;
using System.Linq;

namespace AnticevicApi.DL.Helpers
{
    public class AccessTokenHelper
    {
        public static AccessToken Get(string token)
        {
            using (var db = new MainContext())
            {
                return db.AccessTokens.SingleOrDefault(x => x.Token == token);
            }
        }

        public static int Insert(AccessToken token)
        {
            using (var db = new MainContext())
            {
                db.AccessTokens.Add(token);
                db.SaveChanges();

                return token.Id;
            }
        }
    }
}

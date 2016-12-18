using Microsoft.EntityFrameworkCore;
using System.Linq;

namespace AnticevicApi.BL.Handlers.Security
{
    public class SecurityHandler : Handler<SecurityHandler>, ISecurityHandler
    {
        public SecurityHandler(IHandlerContext<SecurityHandler> context) : base(context)
        {
        }

        public Model.Database.Main.User.User GetUser(string token)
        {
            using (var db = GetMainContext())
            {
                return db.AccessTokens.Include(x => x.User)
                                      .SingleOrDefault(x => x.Token == token)
                                      .User;
            }
        }
    }
}

using ProjectIvy.Common.Helpers;
using ProjectIvy.Model.Database.Main.Security;
using Microsoft.EntityFrameworkCore;
using System.Linq;
using System;

namespace ProjectIvy.Business.Handlers.Security
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

        public string CreateToken(string username, string password)
        {
            using (var db = GetMainContext())
            {
                var user = db.Users.SingleOrDefault(x => x.Username == username);

                if (!PasswordHelper.IsValid(password, user.PasswordHash))
                {
                    throw new NotImplementedException();
                }

                string token = TokenHelper.Generate();

                var accessToken = new AccessToken()
                {
                    Token = token,
                    IsActive = true,
                    UserId = user.Id,
                    ValidFrom = DateTime.Now,
                    ValidUntil = DateTime.Now.AddMonths(1)
                };

                db.AccessTokens.Add(accessToken);
                db.SaveChanges();

                return token;
            }
        }
    }
}

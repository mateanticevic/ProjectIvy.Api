using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using ProjectIvy.Common.Configuration;
using ProjectIvy.Common.Helpers;
using ProjectIvy.Data.DbContexts;
using ProjectIvy.Model.Database.Main.Security;
using System;
using System.Linq;

namespace ProjectIvy.Business.Handlers.Security
{
    public class SecurityHandler : ISecurityHandler
    {
        public SecurityHandler(IOptions<AppSettings> options)
        {
            Settings = options.Value;
        }

        public AppSettings Settings { get; set; }

        public Model.Database.Main.User.User GetUser(string token)
        {
            using (var db = GetMainContext())
            {
                return db.AccessTokens.Include(x => x.User)
                                      .ThenInclude(x => x.DefaultCurrency)
                                      .SingleOrDefault(x => x.Token == token && x.IsActive)
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

        protected MainContext GetMainContext()
        {
            return new MainContext(Settings.ConnectionStrings.Main);
        }
    }
}

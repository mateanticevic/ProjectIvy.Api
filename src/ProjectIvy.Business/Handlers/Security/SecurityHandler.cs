using Microsoft.EntityFrameworkCore;
using ProjectIvy.Common.Helpers;
using ProjectIvy.Data.DbContexts;
using ProjectIvy.Model.Database.Main.Security;
using ProjectIvy.Model.View;
using System;
using System.Linq;
using System.Net;

namespace ProjectIvy.Business.Handlers.Security
{
    public class SecurityHandler : ISecurityHandler
    {

        public Model.Database.Main.User.User GetUser(string token)
        {
            using (var db = GetMainContext())
            {
                return db.AccessTokens.Include(x => x.User)
                                      .ThenInclude(x => x.DefaultCurrency)
                                      .Include(x => x.User)
                                      .SingleOrDefault(x => x.Token == token && x.IsActive)
                                      .User;
            }
        }

        public string CreateToken(string username, string password, RequestContext requestContext)
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
                    ValidUntil = DateTime.Now.AddMonths(1),
                    IpAddress = requestContext.IpAddress,
                    UserAgent = requestContext.UserAgent,
                    OperatingSystem = requestContext.OperatingSystem
                };

                if (IPAddress.TryParse(requestContext.IpAddress, out var ipAddress))
                    accessToken.IpAddressValue = (uint)IPAddress.NetworkToHostOrder((int)BitConverter.ToUInt32(ipAddress.GetAddressBytes(), 0));

                db.AccessTokens.Add(accessToken);
                db.SaveChanges();

                return token;
            }
        }

        protected MainContext GetMainContext()
        {
            return new MainContext(Environment.GetEnvironmentVariable("CONNECTION_STRING_MAIN"));
        }
    }
}

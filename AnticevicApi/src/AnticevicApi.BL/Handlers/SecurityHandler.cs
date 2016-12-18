using AnticevicApi.Common.Helpers;
using AnticevicApi.DL.Helpers;
using System;

namespace AnticevicApi.BL.Handlers
{
    public class SecurityHandler : Handler<SecurityHandler>
    {
        public SecurityHandler(string connectionString, int userId) : base(connectionString, userId)
        {
        }

        public string IssueToken(string username, string password)
        {
            var user = UserHelper.Get(username);

            if(true)
            {
                var at = new Model.Database.Main.Security.AccessToken() { UserId = user.Id, ValidFrom = DateTime.Now, ValidUntil = DateTime.Now.AddMonths(1), IsActive = true, Token = TokenHelper.Generate() };
                AccessTokenHelper.Insert(at);

                return at.Token;
            }
            else
            {
                return null;
            }
        }
    }
}

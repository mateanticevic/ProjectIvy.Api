using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using ProjectIvy.Data.DbContexts;

namespace ProjectIvy.Business.Handlers.Security
{
    public class AccessTokenHandler : IAccessTokenHandler
    {
        public AccessTokenHandler()
        {
        }

        public async Task<IDictionary<string, string>> GetBearers()
        {
            using (var context = GetMainContext())
            {
                return await context.AccessTokens.Where(x => x.Bearer != null)
                                                 .ToDictionaryAsync(x => x.Token, x => x.Bearer);
            }
        }

        private MainContext GetMainContext()
        {
            return new MainContext(Environment.GetEnvironmentVariable("CONNECTION_STRING_MAIN"));
        }
    }
}

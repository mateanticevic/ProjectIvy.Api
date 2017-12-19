using ProjectIvy.DL.DbContexts;
using System.Linq;

namespace ProjectIvy.DL.Extensions.Entities
{
    public static class CurrencyExtensions
    {
        /// <summary>
        /// Returns currency id by code, if code null returns user's default currency id.
        /// </summary>
        /// <param name="context"></param>
        /// <param name="code"></param>
        /// <param name="userId"></param>
        /// <returns></returns>
        public static int GetCurrencyId(this MainContext context, string code, int userId)
        {
            return string.IsNullOrEmpty(code) ? context.Users.SingleOrDefault(x => x.Id == userId).DefaultCurrencyId
                                              : context.Currencies.SingleOrDefault(x => x.Code == code).Id;
        }
    }
}

using AnticevicApi.DL.DbContexts;
using AnticevicApi.Model.View.Currency;
using System.Collections.Generic;
using System.Linq;

namespace AnticevicApi.BL.Handlers
{
    public class CurrencyHandler
    {
        public static IEnumerable<Currency> Get()
        {
            using (var db = new MainContext())
            {
                return db.Currencies.OrderBy(x => x.Name)
                                    .ToList()
                                    .Select(x => new Currency(x));
            }
        }

        public static Currency Get(string code)
        {
            using (var db = new MainContext())
            {
                var entity = db.Currencies.SingleOrDefault(x => x.Code == code);

                return new Currency(entity);
            }
        }
    }
}

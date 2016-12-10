using AnticevicApi.DL.DbContexts;
using System.Collections.Generic;
using System.Linq;
using View = AnticevicApi.Model.View.Currency;

namespace AnticevicApi.BL.Handlers.Currency
{
    public class CurrencyHandler : Handler, ICurrencyHandler
    {
        public IEnumerable<View.Currency> Get()
        {
            using (var db = new MainContext(ConnectionString))
            {
                return db.Currencies.OrderBy(x => x.Name)
                                    .ToList()
                                    .Select(x => new View.Currency(x));
            }
        }

        public View.Currency Get(string code)
        {
            using (var db = new MainContext(ConnectionString))
            {
                var entity = db.Currencies.SingleOrDefault(x => x.Code == code);

                return new View.Currency(entity);
            }
        }
    }
}

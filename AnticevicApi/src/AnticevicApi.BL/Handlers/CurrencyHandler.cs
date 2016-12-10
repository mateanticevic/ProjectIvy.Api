using AnticevicApi.DL.DbContexts;
using AnticevicApi.Model.View.Currency;
using System.Collections.Generic;
using System.Linq;

namespace AnticevicApi.BL.Handlers
{
    public class CurrencyHandler : Handler
    {
        public CurrencyHandler(string connectionString, int userId) : base(connectionString, userId)
        {

        }

        public IEnumerable<Currency> Get()
        {
            using (var db = new MainContext(ConnectionString))
            {
                return db.Currencies.OrderBy(x => x.Name)
                                    .ToList()
                                    .Select(x => new Currency(x));
            }
        }

        public Currency Get(string code)
        {
            using (var db = new MainContext(ConnectionString))
            {
                var entity = db.Currencies.SingleOrDefault(x => x.Code == code);

                return new Currency(entity);
            }
        }
    }
}

﻿using System.Collections.Generic;
using System.Linq;
using View = ProjectIvy.Model.View.Currency;

namespace ProjectIvy.BL.Handlers.Currency
{
    public class CurrencyHandler : Handler<CurrencyHandler>, ICurrencyHandler
    {
        public CurrencyHandler(IHandlerContext<CurrencyHandler> context) : base(context)
        {
        }

        public IEnumerable<View.Currency> Get()
        {
            using (var db = GetMainContext())
            {
                return db.Currencies.OrderBy(x => x.Name)
                                    .ToList()
                                    .Select(x => new View.Currency(x));
            }
        }

        public View.Currency Get(string code)
        {
            using (var db = GetMainContext())
            {
                var entity = db.Currencies.SingleOrDefault(x => x.Code == code);

                return new View.Currency(entity);
            }
        }
    }
}
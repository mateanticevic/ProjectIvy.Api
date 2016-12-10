using System.Collections.Generic;
using View = AnticevicApi.Model.View.Currency;

namespace AnticevicApi.BL.Handlers.Currency
{
    public interface ICurrencyHandler : IHandler
    {
        IEnumerable<View.Currency> Get();

        View.Currency Get(string code);
    }
}

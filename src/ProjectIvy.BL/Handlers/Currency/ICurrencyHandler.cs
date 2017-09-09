using System.Collections.Generic;
using View = ProjectIvy.Model.View.Currency;

namespace ProjectIvy.BL.Handlers.Currency
{
    public interface ICurrencyHandler : IHandler
    {
        IEnumerable<View.Currency> Get();

        View.Currency Get(string code);
    }
}

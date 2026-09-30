using System.Collections.Generic;
using System.Threading.Tasks;
using View = ProjectIvy.Model.View.Currency;

namespace ProjectIvy.Business.Handlers.Currency;

public interface ICurrencyHandler : IHandler
{
    Task<IEnumerable<View.Currency>> Get();

    Task<View.Currency> Get(string code);
}

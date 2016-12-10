using System.Collections.Generic;
using View = AnticevicApi.Model.View.Airport;

namespace AnticevicApi.BL.Handlers.Airport
{
    public interface IAirportHandler : IHandler
    {
        IEnumerable<View.Airport> Get(bool onlyVisited = false);
    }
}

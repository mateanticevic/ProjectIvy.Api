using System.Collections.Generic;
using View = AnticevicApi.Model.View.Card;

namespace AnticevicApi.BL.Handlers.Card
{
    public interface ICardHandler : IHandler
    {
        IEnumerable<View.Card> GetCards();
    }
}

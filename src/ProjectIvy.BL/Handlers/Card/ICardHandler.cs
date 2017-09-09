using System.Collections.Generic;
using View = ProjectIvy.Model.View.Card;

namespace ProjectIvy.BL.Handlers.Card
{
    public interface ICardHandler : IHandler
    {
        IEnumerable<View.Card> GetCards();
    }
}

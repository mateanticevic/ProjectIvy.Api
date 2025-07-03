using System.Collections.Generic;
using ProjectIvy.Model.Binding.Card;
using View = ProjectIvy.Model.View.Card;

namespace ProjectIvy.Business.Handlers.Card;

public interface ICardHandler : IHandler
{
    IEnumerable<View.Card> GetCards(CardGetBinding binding);
}

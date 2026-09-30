using System.Collections.Generic;
using System.Threading.Tasks;
using ProjectIvy.Model.Binding.Card;
using View = ProjectIvy.Model.View.Card;

namespace ProjectIvy.Business.Handlers.Card;

public interface ICardHandler : IHandler
{
    Task<IEnumerable<View.Card>> GetCards(CardGetBinding binding);
}

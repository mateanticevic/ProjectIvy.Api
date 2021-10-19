using ProjectIvy.Data.Extensions;
using ProjectIvy.Model.Binding.Card;
using System.Collections.Generic;
using System.Linq;
using View = ProjectIvy.Model.View.Card;

namespace ProjectIvy.Business.Handlers.Card
{
    public class CardHandler : Handler<CardHandler>, ICardHandler
    {
        public CardHandler(IHandlerContext<CardHandler> context) : base(context)
        {
        }

        public IEnumerable<View.Card> GetCards(CardGetBinding binding)
        {
            using (var context = GetMainContext())
            {
                return context.Cards.WhereUser(UserId.Value)
                                    .WhereIf(binding.IsActive.HasValue, x => x.IsActive == binding.IsActive.Value)
                                    .Select(x => new View.Card(x))
                                    .ToList();
            }
        }
    }
}

using AnticevicApi.DL.Extensions;
using System.Collections.Generic;
using System.Linq;
using View = AnticevicApi.Model.View.Card;

namespace AnticevicApi.BL.Handlers.Card
{
    public class CardHandler : Handler<CardHandler>, ICardHandler
    {
        public CardHandler(IHandlerContext<CardHandler> context) : base(context)
        {
        }

        public IEnumerable<View.Card> GetCards()
        {
            using (var context = GetMainContext())
            {
                return context.Cards.WhereUser(User)
                                    .Select(x => new View.Card(x))
                                    .ToList();
            }
        }
    }
}

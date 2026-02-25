using System.Linq;
using ProjectIvy.Data.Extensions;
using ProjectIvy.Model.Binding.Card;
using Microsoft.EntityFrameworkCore;
using View = ProjectIvy.Model.View.Card;

namespace ProjectIvy.Business.Handlers.Card;

public class CardHandler : Handler<CardHandler>, ICardHandler
{
    public CardHandler(IHandlerContext<CardHandler> context) : base(context)
    {
    }

    public IEnumerable<View.Card> GetCards(CardGetBinding binding)
    {
        using var context = GetMainContext();
        return context.Cards.WhereUser(UserId)
                            .Include(x => x.Bank)
                            .Include(x => x.CardType)
                            .WhereIf(binding.HasExpired.HasValue, x => (binding.HasExpired.Value && x.Expires <= DateTime.Now) || (!binding.HasExpired.Value && x.Expires >= DateTime.Now))
                            .WhereIf(binding.IsActive.HasValue, x => x.IsActive == binding.IsActive.Value)
                            .WhereIf(!string.IsNullOrWhiteSpace(binding.LastFourDigits), x => x.LastFourDigits == binding.LastFourDigits)
                            .OrderByDescending(x => x.Expires)
                            .Select(x => new View.Card(x))
                            .ToList();
    }
}

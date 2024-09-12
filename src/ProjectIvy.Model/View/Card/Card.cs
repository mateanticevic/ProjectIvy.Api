using DatabaseModel = ProjectIvy.Model.Database.Main.Finance;

namespace ProjectIvy.Model.View.Card
{
    public class Card
    {
        public Card(DatabaseModel.Card x)
        {
            Id = x.ValueId;
            Bank = x.Bank is null ? null : new Bank.Bank(x.Bank);
            Type = x.CardType is null ? null : new CardType(x.CardType);
            HasExpired = x.Expires > DateTime.Now;
            Name = x.Name;
            Expires = x.Expires;
            Issued = x.Issued;
            LastFourDigits = x.LastFourDigits;
        }

        public string Id { get; set; }

        public Bank.Bank Bank { get; set; }

        public CardType Type { get; set; }

        public string Name { get; set; }

        public bool HasExpired { get; set; }

        public DateTime Expires { get; set; }

        public DateTime Issued { get; set; }

        public string LastFourDigits { get; set; }
    }
}

using DatabaseModel = AnticevicApi.Model.Database.Main.Finance;
using System;

namespace AnticevicApi.Model.View.Card
{
    public class Card
    {
        public Card(DatabaseModel.Card x)
        {
            Id = x.ValueId;
            Name = x.Name;
            Expires = x.Expires;
            Issued = x.Issued;
            LastFourDigits = x.LastFourDigits;
        }

        public string Id { get; set; }

        public string Name { get; set; }

        public DateTime Expires { get; set; }

        public DateTime Issued { get; set; }

        public string LastFourDigits { get; set; }
    }
}

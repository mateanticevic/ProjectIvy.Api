﻿using System;
using DatabaseModel = ProjectIvy.Model.Database.Main.Finance;

namespace ProjectIvy.Model.View.Card
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

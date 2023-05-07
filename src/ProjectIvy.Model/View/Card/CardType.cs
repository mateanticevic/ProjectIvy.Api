using System;
using DatabaseModel = ProjectIvy.Model.Database.Main.Finance;

namespace ProjectIvy.Model.View.Card
{
	public class CardType
	{
        public CardType(DatabaseModel.CardType x)
        {
            Id = x.ValueId;
            Name = x.Name;
        }

        public string Id { get; set; }

		public string Name { get; set; }
    }
}


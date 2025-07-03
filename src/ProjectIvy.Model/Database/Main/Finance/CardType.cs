using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ProjectIvy.Model.Database.Main.Finance;

[Table(nameof(CardType), Schema = nameof(Finance))]
public class CardType : IHasValueId, IHasName
{
	[Key]
	public int Id { get; set; }

	public string ValueId { get; set; }

	public string Name { get; set; }
}

using System.ComponentModel.DataAnnotations.Schema;

namespace ProjectIvy.Model.Database.Main.Tracking;

[Table(nameof(LocationType), Schema = nameof(Tracking))]
public class LocationType : IHasValueId
{
	public int Id { get; set; }

	public string ValueId { get; set; }

	public string Name { get; set; }
}

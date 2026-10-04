using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ProjectIvy.Model.Database.Main.Tracking;

[Table(nameof(TrackingView), Schema = nameof(Tracking))]
public class TrackingView : UserEntity, IHasValueId, IHasName
{
    [Key]
    public int Id { get; set; }

    public string ValueId { get; set; }

    public string Name { get; set; }

    public DateTime From { get; set; }

    public DateTime To { get; set; }
}

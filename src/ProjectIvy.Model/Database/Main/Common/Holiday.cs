using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ProjectIvy.Model.Database.Main.Common;

[Table(nameof(Holiday), Schema = nameof(Common))]
public class Holiday
{
    [Key]
    public int Id { get; set; }

    public int CountryId { get; set; }

    public DateTime Date { get; set; }

    public Country Country { get; set; }
}

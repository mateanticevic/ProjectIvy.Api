using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ProjectIvy.Model.Database.Main.Common;

[Table(nameof(Holiday), Schema = nameof(Common))]
public class Holiday
{
    public Country Country { get; set; }

    public int CountryId { get; set; }

    public DateTime Date { get; set; }

    [Key]
    public int Id { get; set; }
}

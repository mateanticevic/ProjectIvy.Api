using System.ComponentModel.DataAnnotations.Schema;

namespace ProjectIvy.Model.Database.Main.Travel;

[Table(nameof(Stay), Schema = nameof(Travel))]
public class Stay : UserEntity
{
    public int Id { get; set; }

    public DateTime Date { get; set; }

    public int? CityId { get; set; }

    public int CountryId { get; set; }

    public Common.City City { get; set; }

    public Common.Country Country { get; set; }
}
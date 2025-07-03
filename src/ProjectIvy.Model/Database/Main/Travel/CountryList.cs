using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ProjectIvy.Model.Database.Main.Travel;

[Table(nameof(CountryList), Schema = nameof(Travel))]
public class CountryList : IHasValueId, IHasName
{
    [Key]
    public int Id { get; set; }

    public string ValueId { get; set; }

    public string Name { get; set; }

    public int? UserId { get; set; }

    public ICollection<CountryListCountry> Countries { get; set; }
}

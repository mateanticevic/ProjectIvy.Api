using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using ProjectIvy.Model.Database.Main.Common;

namespace ProjectIvy.Model.Database.Main.Finance;

[Table(nameof(Vendor), Schema = nameof(Finance))]
public class Vendor : IHasValueId
{
    public City City { get; set; }

    public int? CityId { get; set; }

    public ICollection<Expense> Expenses { get; set; }

    [Key]
    public int Id { get; set; }

    public string Name { get; set; }

    public string ValueId { get; set; }
}

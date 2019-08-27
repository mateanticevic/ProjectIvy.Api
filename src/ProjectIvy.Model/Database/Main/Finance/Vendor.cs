using ProjectIvy.Model.Database.Main.Common;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ProjectIvy.Model.Database.Main.Finance
{
    [Table(nameof(Vendor), Schema = nameof(Finance))]
    public class Vendor : IHasValueId
    {
        [Key]
        public int Id { get; set; }

        public string ValueId { get; set; }

        public string Name { get; set; }

        public int? CityId { get; set; }

        public City City { get; set; }

        public ICollection<Expense> Expenses { get; set; }
    }
}

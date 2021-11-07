using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ProjectIvy.Model.Database.Main.Finance
{
    [Table(nameof(Account), Schema = nameof(Finance))]
    public class Account : IHasValueId, IHasName
    {
        [Key]
        public int Id { get; set; }

        public string ValueId { get; set; }

        public string Name { get; set; }
    }
}

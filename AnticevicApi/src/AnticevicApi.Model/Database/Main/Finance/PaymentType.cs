using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace AnticevicApi.Model.Database.Main.Finance
{
    [Table(nameof(PaymentType), Schema = nameof(Finance))]
    public class PaymentType : IHasValueId, IHasName
    {
        [Key]
        public int Id { get; set; }

        public string ValueId { get; set; }

        public string Name { get; set; }

        public string Description { get; set; }
    }
}

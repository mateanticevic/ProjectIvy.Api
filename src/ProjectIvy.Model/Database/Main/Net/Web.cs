using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.Collections.Generic;

namespace ProjectIvy.Model.Database.Main.Net
{
    [Table(nameof(Web), Schema = "Net")]
    public class Web : IHasValueId, IHasName
    {
        [Key]
        public int Id { get; set; }

        public string ValueId { get; set; }

        public string Name { get; set; }

        public ICollection<Domain> Domains { get; set; }
    }
}

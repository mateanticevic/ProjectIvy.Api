using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.Collections.Generic;

namespace AnticevicApi.Model.Database.Main.Net
{
    [Table(nameof(WebSite), Schema = "Net")]
    public class WebSite : IHasValueId, IHasName
    {
        [Key]
        public int Id { get; set; }

        public string ValueId { get; set; }

        public string Name { get; set; }

        public ICollection<Domain> Domains { get; set; }
    }
}

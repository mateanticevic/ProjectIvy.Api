using ProjectIvy.Model.Database.Main.Log;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ProjectIvy.Model.Database.Main.Net
{
    [Table(nameof(Domain), Schema = nameof(Net))]
    public class Domain : IHasValueId
    {
        [Key]
        public int Id { get; set; }

        public string ValueId { get; set; }

        public int WebId { get; set; }

        public Web Web { get; set; }

        public ICollection<BrowserLog> BrowserLogs { get; set; }
    }
}

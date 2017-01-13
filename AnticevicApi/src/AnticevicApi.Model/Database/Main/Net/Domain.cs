using AnticevicApi.Model.Database.Main.Log;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace AnticevicApi.Model.Database.Main.Net
{
    [Table(nameof(Domain), Schema = "Net")]
    public class Domain : IHasValueId
    {
        [Key]
        public int Id { get; set; }

        public string ValueId { get; set; }

        public int WebSiteId { get; set; }

        public WebSite WebSite { get; set; }

        public ICollection<BrowserLog> BrowserLogs { get; set; }
    }
}

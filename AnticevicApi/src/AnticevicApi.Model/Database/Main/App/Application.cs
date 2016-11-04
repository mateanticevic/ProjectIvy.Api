using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.Collections.Generic;

namespace AnticevicApi.Model.Database.Main.App
{
    [Table("Application", Schema = "App")]
    public class Application : IHasValueId
    {
        [Key]
        public int Id { get; set; }
        public string ValueId { get; set; }
        public string Name { get; set; }
        public ICollection<ApplicationSetting> Settings { get; set; }
    }
}

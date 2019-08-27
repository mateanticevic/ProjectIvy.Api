using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ProjectIvy.Model.Database.Main.App
{
    [Table("ApplicationSetting", Schema = "App")]
    public class ApplicationSetting
    {
        [Key]
        public int Id { get; set; }

        public string Key { get; set; }

        public string Value { get; set; }

        public Application Application { get; set; }
    }
}

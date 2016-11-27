using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace AnticevicApi.Model.Database.Main.Org
{
    [Table("TaskPriority", Schema = "Org")]
    public class TaskPriority : IHasValueId, IHasName
    {
        [Key]
        public int Id { get; set; }
        public string ValueId { get; set; }
        public string Name { get; set; }
        public int Weight { get; set; }
    }
}

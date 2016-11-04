using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace AnticevicApi.Model.Database.Main.Org
{
    [Table("TaskType", Schema = "Org")]
    public class TaskType : IHasValueId, IHasName
    {
        [Key]
        public int Id { get; set; }
        public string ValueId { get; set; }
        public string Name { get; set; }
    }
}

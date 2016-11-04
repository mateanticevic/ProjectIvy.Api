using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace AnticevicApi.Model.Database.Main.Org
{
    [Table("TaskStatus", Schema = "Org")]
    public class TaskStatus : IHasValueId, IHasName
    {
        [Key]
        public int Id { get; set; }
        public string ValueId { get; set; }
        public string Name { get; set; }
    }
}

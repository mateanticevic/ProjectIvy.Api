using System.ComponentModel.DataAnnotations.Schema;

namespace AnticevicApi.Model.Database.Main.Org
{
    [Table("RelatedTask", Schema = "Org")]
    public class RelatedTask
    {
        public int TaskId { get; set; }
        public int RelatedId { get; set; }

        public Task Task { get; set; }
        public Task Related { get; set; }
    }
}

using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ProjectIvy.Model.Database.Main.Org
{
    [Table("Task", Schema = "Org")]
    public class Task : IHasValueId, IHasName
    {
        [Key]
        public int Id { get; set; }

        public string ValueId { get; set; }

        public string Name { get; set; }

        public string Description { get; set; }

        public DateTime Created { get; set; }

        public DateTime? DueDate { get; set; }

        public DateTime Modified { get; set; }

        public int ProjectId { get; set; }

        public int TaskTypeId { get; set; }

        public ICollection<RelatedTask> Related { get; set; }

        public ICollection<TaskChange> Changes { get; set; }

        public ICollection<RelatedTask> WhichRelate { get; set; }

        public Project Project { get; set; }

        public TaskType Type { get; set; }
    }
}

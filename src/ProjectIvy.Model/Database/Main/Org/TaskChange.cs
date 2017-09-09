using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System;

namespace ProjectIvy.Model.Database.Main.Org
{
    [Table("TaskChange", Schema = "Org")]
    public class TaskChange
    {
        [Key]
        public int Id { get; set; }

        public int TaskId { get; set; }

        public int TaskPriorityId { get; set; }

        public int TaskStatusId { get; set; }

        public DateTime Timestamp { get; set; }

        public TaskPriority Priority { get; set; }

        public TaskStatus Status { get; set; }

        public Task Task { get; set; }
    }
}

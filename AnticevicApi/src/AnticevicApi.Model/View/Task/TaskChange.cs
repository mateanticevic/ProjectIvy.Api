using AnticevicApi.Extensions.BuiltInTypes;
using System;
using DatabaseModel = AnticevicApi.Model.Database.Main;

namespace AnticevicApi.Model.View.Task
{
    public class TaskChange
    {
        public TaskChange(DatabaseModel.Org.TaskChange x)
        {
            Timestamp = x.Timestamp;
            Priority = x.Priority.ConvertTo(y => new TaskPriority(y));
            Status = x.Status.ConvertTo(y => new TaskStatus(y));
        }

        public DateTime Timestamp { get; set; }
        public TaskPriority Priority { get; set; }
        public TaskStatus Status { get; set; }
    }
}

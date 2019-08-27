using ProjectIvy.Common.Extensions;
using System;
using DatabaseModel = ProjectIvy.Model.Database.Main;

namespace ProjectIvy.Model.View.Task
{
    public class Change
    {
        public Change(DatabaseModel.Org.TaskChange x)
        {
            Timestamp = x.Timestamp;
            Priority = x.Priority.ConvertTo(y => new Priority(y));
            Status = x.Status.ConvertTo(y => new Status(y));
        }

        public DateTime Timestamp { get; set; }

        public Priority Priority { get; set; }

        public Status Status { get; set; }
    }
}

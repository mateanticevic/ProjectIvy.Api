using DatabaseModel = AnticevicApi.Model.Database.Main;
using System.Collections.Generic;
using System.Linq;
using System;

namespace AnticevicApi.Model.View.Task
{
    public class Task
    {
        public Task(DatabaseModel.Org.Task x)
        {
            Changes = x.Changes.OrderByDescending(y => y.Timestamp).Select(y => new TaskChange(y));
            Created = x.Created;
            Description = x.Description;
            DueDate = x.DueDate;
            Modified = x.Modified;
            Name = x.Name;
            ValueId = x.ValueId;
            //Related = x.Related.Select(y => new Task(y.Related));
        }

        public DateTime Created { get; set; }
        public DateTime Modified { get; set; }
        public DateTime? DueDate { get; set; }
        public IEnumerable<TaskChange> Changes { get; set; }
        public IEnumerable<Task> Related { get; set; }
        public string Description { get; set; }
        public string Name { get; set; }
        public string ValueId { get; set; }
    }
}

using AnticevicApi.Extensions.BuiltInTypes;
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
            Changes = x.Changes?.OrderByDescending(y => y.Timestamp).Select(y => new Change(y));
            Created = x.Created;
            Description = x.Description;
            DueDate = x.DueDate;
            Modified = x.Modified;
            Name = x.Name;
            Type = x.Type.ConvertTo(y => new Type(y));
            Id = x.ValueId;
        }

        public Task(DatabaseModel.Org.Task t, DatabaseModel.Org.TaskChange c, string projectId) : this(t)
        {
            LastChange = new Change(c);
            ProjectId = projectId;
        }

        public string Id { get; set; }

        public DateTime Created { get; set; }

        public DateTime Modified { get; set; }

        public DateTime? DueDate { get; set; }

        public IEnumerable<Task> Related { get; set; }

        public IEnumerable<Change> Changes { get; set; }

        public string Description { get; set; }

        public string Name { get; set; }

        public string ProjectId { get; set; }

        public Change LastChange { get; set; }

        public Type Type { get; set; }
    }
}

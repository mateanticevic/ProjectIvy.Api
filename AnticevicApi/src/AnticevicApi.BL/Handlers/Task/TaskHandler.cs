using AnticevicApi.BL.MapExtensions;
using AnticevicApi.DL.DbContexts;
using AnticevicApi.DL.Extensions;
using AnticevicApi.Model.Binding.Task;
using AnticevicApi.Model.Constants.Database;
using Database = AnticevicApi.Model.Database.Main;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;
using System;
using View = AnticevicApi.Model.View.Task;

namespace AnticevicApi.BL.Handlers.Task
{
    public class TaskHandler : Handler<TaskHandler>, ITaskHandler
    {
        public TaskHandler(IHandlerContext<TaskHandler> context) : base(context)
        {

        }

        public string Create(TaskBinding binding)
        {
            using (var db = GetMainContext())
            {
                var entity = binding.ToEntity(db);
                entity.Created = DateTime.Now;
                entity.Modified = DateTime.Now;
                entity.ValueId = db.Tasks.NextValueId(entity.ProjectId).ToString();

                var taskChange = new Database.Org.TaskChange()
                {
                    Timestamp = DateTime.Now,
                    Task = entity,
                    TaskStatusId = TaskStatuses.New.Key,
                    TaskPriorityId = TaskPriorities.GetId(binding.PriorityId)
                };

                db.Tasks.Add(entity);
                db.TaskChanges.Add(taskChange);
                db.SaveChanges();

                return entity.ValueId;
            }
        }

        public IEnumerable<View.Task> Get(string projectValueId)
        {
            using (var db = GetMainContext())
            {
                return db.Projects.Include(x => x.Tasks)
                                  .SingleOrDefault(projectValueId)
                                  .Tasks
                                  .ToList()
                                  .Select(x => new View.Task(x));
            }
        }

        public View.Task Get(string projectValueId, string taskValueId)
        {
            using (var db = GetMainContext())
            {
                int projectId = db.Projects.WhereUser(UserId).SingleOrDefault(x => x.ValueId == projectValueId).Id;
                var task = db.Tasks.Include(x => x.Related)
                                   .Include(x => x.Changes)
                                   .SingleOrDefault(x => x.ValueId == taskValueId && x.ProjectId == projectId);

                return new View.Task(task);
            }
        }

        public IEnumerable<View.Task> Get(string statusValueId, string priorityValueId, string typeValueId)
        {
            using (var db = GetMainContext())
            {
                var tasks = db.Projects.WhereUser(UserId)
                                       .Join(db.Tasks, x => x.Id, x => x.ProjectId, (Project, Task) => new { Project, Task })
                                       .GroupJoin(db.TaskChanges, x => x.Task.Id, x => x.TaskId, (tp, t2) => new { Task = tp.Task, Project = tp.Project, LastChange = t2.OrderByDescending(y => y.Timestamp).FirstOrDefault() })
                                       .Join(db.TaskStatuses, x => x.LastChange.TaskStatusId, x => x.Id, (tc, Status) => new { tc.Project, tc.LastChange, tc.Task, Status })
                                       .Join(db.TaskPriorities, x => x.LastChange.TaskPriorityId, x => x.Id, (t, Priority) => new { t.Project, t.LastChange, t.Task, t.Status, Priority })
                                       .Join(db.TaskTypes, x => x.Task.TaskTypeId, x => x.Id, (t, Type) => new { t.Project, t.LastChange, t.Task, t.Priority, Type, t.Status });

                tasks = string.IsNullOrEmpty(statusValueId) ? tasks : tasks.Where(x => x.Status.ValueId == statusValueId);
                tasks = string.IsNullOrEmpty(priorityValueId) ? tasks : tasks.Where(x => x.Priority.ValueId == priorityValueId);
                tasks = string.IsNullOrEmpty(typeValueId) ? tasks : tasks.Where(x => x.Task.Type.ValueId == typeValueId);

                tasks = tasks.OrderByDescending(x => x.Task.DueDate)
                             .ThenByDescending(x => x.Task.Created);

                return tasks.ToList().Select(x => new View.Task(x.Task, x.LastChange, x.Project.ValueId));
            }
        }
    }
}

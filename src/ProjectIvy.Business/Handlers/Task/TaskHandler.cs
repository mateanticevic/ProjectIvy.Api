using Microsoft.EntityFrameworkCore;
using ProjectIvy.Business.MapExtensions;
using ProjectIvy.Data.Extensions;
using ProjectIvy.Model.Binding.Task;
using ProjectIvy.Model.Constants.Database;
using System;
using System.Collections.Generic;
using System.Linq;
using Database = ProjectIvy.Model.Database.Main;
using View = ProjectIvy.Model.View.Task;

namespace ProjectIvy.Business.Handlers.Task
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

        public void CreateChange(TaskChangeBinding binding)
        {
            using (var db = GetMainContext())
            {
                int taskId = db.Projects.Include(x => x.Tasks)
                                        .WhereUser(User.Id)
                                        .SingleOrDefault(x => x.ValueId == binding.ProjectId)
                                        .Tasks
                                        .SingleOrDefault(x => x.ValueId == binding.TaskId)
                                        .Id;

                var lastTaskChange = db.TaskChanges.Where(x => x.TaskId == taskId)
                                                   .OrderByDescending(x => x.Timestamp)
                                                   .FirstOrDefault();

                int priorityId = lastTaskChange.TaskPriorityId;
                int statusId = lastTaskChange.TaskStatusId;

                priorityId = string.IsNullOrEmpty(binding.PriorityId) ? priorityId : db.TaskPriorities.SingleOrDefault(x => x.ValueId == binding.PriorityId).Id;
                statusId = string.IsNullOrEmpty(binding.StatusId) ? statusId : db.TaskStatuses.SingleOrDefault(x => x.ValueId == binding.StatusId).Id;

                var taskChange = new Database.Org.TaskChange()
                {
                    TaskId = taskId,
                    TaskPriorityId = priorityId,
                    TaskStatusId = statusId,
                    Timestamp = DateTime.Now
                };

                db.TaskChanges.Add(taskChange);
                db.SaveChanges();
            }
        }

        public void Delete(string projectValueId, string taskValueId)
        {
            using (var context = GetMainContext())
            {
                int projectId = context.Projects.GetId(projectValueId).Value;
                var task = context.Tasks.SingleOrDefault(x => x.ProjectId == projectId && x.ValueId == taskValueId);
                var taskChanges = context.TaskChanges.Where(x => x.TaskId == task.Id);

                context.TaskChanges.RemoveRange(taskChanges);
                context.Tasks.Remove(task);

                context.SaveChanges();
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
                int projectId = db.Projects.WhereUser(User.Id).SingleOrDefault(x => x.ValueId == projectValueId).Id;
                var task = db.Tasks.Include(x => x.Related)
                                   .Include(x => x.Changes)
                                   .SingleOrDefault(x => x.ValueId == taskValueId && x.ProjectId == projectId);

                return new View.Task(task);
            }
        }

        public IEnumerable<View.Task> Get(TaskGetBinding binding)
        {
            using (var db = GetMainContext())
            {
                var projectValueIds = string.IsNullOrEmpty(binding.Projects) ? null : binding.Projects.Split(',');

                var tasks = db.Projects.WhereUser(User.Id)
                                       .Join(db.Tasks, x => x.Id, x => x.ProjectId, (Project, Task) => new { Project, Task })
                                       .GroupJoin(db.TaskChanges, x => x.Task.Id, x => x.TaskId, (tp, t2) => new { Task = tp.Task, Project = tp.Project, LastChange = t2.OrderByDescending(y => y.Timestamp).FirstOrDefault() })
                                       .Join(db.TaskStatuses, x => x.LastChange.TaskStatusId, x => x.Id, (tc, Status) => new { tc.Project, tc.LastChange, tc.Task, Status })
                                       .Join(db.TaskPriorities, x => x.LastChange.TaskPriorityId, x => x.Id, (t, Priority) => new { t.Project, t.LastChange, t.Task, t.Status, Priority })
                                       .Join(db.TaskTypes, x => x.Task.TaskTypeId, x => x.Id, (t, Type) => new { t.Project, t.LastChange, t.Task, t.Priority, Type, t.Status });

                tasks = string.IsNullOrEmpty(binding.Status) ? tasks : tasks.Where(x => x.Status.ValueId == binding.Status);
                tasks = string.IsNullOrEmpty(binding.Priority) ? tasks : tasks.Where(x => x.Priority.ValueId == binding.Priority);
                tasks = string.IsNullOrEmpty(binding.Type) ? tasks : tasks.Where(x => x.Task.Type.ValueId == binding.Type);
                tasks = string.IsNullOrEmpty(binding.Projects) ? tasks : tasks.Where(x => projectValueIds.Contains(x.Project.ValueId));

                tasks = tasks.OrderByDescending(x => x.Task.DueDate)
                             .ThenByDescending(x => x.Task.Created);

                return tasks.ToList().Select(x => new View.Task(x.Task, x.LastChange, x.Project.ValueId));
            }
        }

        public IEnumerable<View.Priority> GetPriorities()
        {
            using (var db = GetMainContext())
            {
                return db.TaskPriorities.OrderBy(x => x.Name)
                                        .ToList()
                                        .Select(x => new View.Priority(x));
            }
        }

        public IEnumerable<View.Status> GetStatuses()
        {
            using (var db = GetMainContext())
            {
                return db.TaskStatuses.OrderBy(x => x.Name)
                                        .ToList()
                                        .Select(x => new View.Status(x));
            }
        }

        public IEnumerable<View.Type> GetTypes()
        {
            using (var db = GetMainContext())
            {
                return db.TaskTypes.OrderBy(x => x.Name)
                                        .ToList()
                                        .Select(x => new View.Type(x));
            }
        }
    }
}

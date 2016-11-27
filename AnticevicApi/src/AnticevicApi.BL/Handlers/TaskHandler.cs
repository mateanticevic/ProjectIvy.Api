using AnticevicApi.BL.MapExtensions;
using AnticevicApi.DL.DbContexts;
using AnticevicApi.DL.Extensions;
using AnticevicApi.DL.Helpers;
using AnticevicApi.Model.Binding.Task;
using AnticevicApi.Model.Constants.Database;
using AnticevicApi.Model.View.Task;
using Database = AnticevicApi.Model.Database.Main;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;
using System;

namespace AnticevicApi.BL.Handlers
{
    public class TaskHandler : Handler
    {
        public TaskHandler(int userId) : base(userId)
        {

        }

        public string Create(TaskBinding binding)
        {
            using (var db = new MainContext())
            {
                var entity = binding.ToEntity();
                entity.Created = DateTime.Now;
                entity.Modified = DateTime.Now;
                entity.ValueId = (Convert.ToInt32(TaskHelper.LastValueId(entity.ProjectId)) + 1).ToString();

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

        public IEnumerable<Task> Get(string projectValueId)
        {
            using (var db = new MainContext())
            {
                return db.Projects.Include(x => x.Tasks)
                                  .SingleOrDefault(projectValueId)
                                  .Tasks
                                  .ToList()
                                  .Select(x => new Task(x));
            }
        }

        public Task Get(string projectValueId, string taskValueId)
        {
            using (var db = new MainContext())
            {
                int projectId = db.Projects.WhereUser(UserId).SingleOrDefault(x => x.ValueId == projectValueId).Id;
                var task = db.Tasks.Include(x => x.Related)
                                   .Include(x => x.Changes)
                                   .SingleOrDefault(x => x.ValueId == taskValueId && x.ProjectId == projectId);

                return new Task(task);
            }
        }

        public IEnumerable<Task> Get(string statusValueId, string priorityValueId, string typeValueId)
        {
            using (var db = new MainContext())
            {
                int? priorityId = db.TaskPriorities.SingleOrDefault(x => x.ValueId == priorityValueId)?.Id;
                int? statusId = db.TaskStatuses.SingleOrDefault(x => x.ValueId == statusValueId)?.Id;
                int? typeId = db.TaskTypes.SingleOrDefault(x => x.ValueId == typeValueId)?.Id;

                db.TaskStatuses.SingleOrDefault(x => x.ValueId == "new");

                var q = db.Projects.WhereUser(UserId)
                                   .Join(db.Tasks, x => x.Id, x => x.ProjectId, (project, task) => new { Project = project, Task = task })
                                   .GroupJoin(db.TaskChanges, x => x.Task.Id, x => x.TaskId, (tp, t2) => new { Task = tp.Task, Project = tp.Project, LastChange = t2.OrderByDescending(y => y.Timestamp).FirstOrDefault() });

                q = priorityId.HasValue ? q.Where(x => x.LastChange.TaskPriorityId == priorityId) : q;
                q = statusId.HasValue ? q.Where(x => x.LastChange.TaskStatusId == statusId) : q;
                q = typeId.HasValue ? q.Where(x => x.Task.TaskTypeId == typeId) : q;

                q = q.OrderByDescending(x => x.Task.DueDate)
                     .ThenByDescending(x => x.Task.Created);

                return q.ToList().Select(x => new Task(x.Task, x.LastChange, x.Project.ValueId));
            }
        }
    }
}

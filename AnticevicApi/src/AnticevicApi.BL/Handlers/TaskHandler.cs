using AnticevicApi.DL.DbContexts;
using AnticevicApi.DL.Extensions;
using AnticevicApi.Model.View.Task;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;

namespace AnticevicApi.BL.Handlers
{
    public class TaskHandler : Handler
    {
        public TaskHandler(int userId) : base(userId)
        {

        }

        public IEnumerable<Task> GetTasks(string projectValueId)
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

        public IEnumerable<Task> GetBy(string statusValueId, string priorityValueId, string typeValueId)
        {
            using (var db = new MainContext())
            {
                int? priorityId = db.TaskPriorities.SingleOrDefault(x => x.ValueId == priorityValueId)?.Id;
                int? statusId = db.TaskStatuses.SingleOrDefault(x => x.ValueId == statusValueId)?.Id;
                int? typeId = db.TaskTypes.SingleOrDefault(x => x.ValueId == typeValueId)?.Id;

                var q = db.Projects.WhereUser(UserId)
                                   .Join(db.Tasks, x => x.Id, x => x.ProjectId, (project, task) => task)
                                   .GroupJoin(db.TaskChanges, x => x.Id, x => x.TaskId, (t1, t2) => new { Task = t1, LastChange = t2.OrderByDescending(y => y.Timestamp).FirstOrDefault() });

                q = priorityId.HasValue ? q.Where(x => x.LastChange.TaskPriorityId == priorityId) : q;
                q = statusId.HasValue ? q.Where(x => x.LastChange.TaskStatusId == statusId) : q;
                q = typeId.HasValue ? q.Where(x => x.Task.TaskTypeId == typeId) : q;

                return q.ToList().Select(x => new Task(x.Task, x.LastChange));
            }
        }
    }
}

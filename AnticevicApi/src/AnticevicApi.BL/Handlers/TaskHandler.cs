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
    }
}

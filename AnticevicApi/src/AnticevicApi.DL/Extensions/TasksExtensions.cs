using AnticevicApi.Model.Database.Main.Org;
using Microsoft.EntityFrameworkCore;
using System.Linq;
using System;

namespace AnticevicApi.DL.Extensions
{
    public static class TasksExtensions
    {
        public static string LastValueId(this DbSet<Task> set, int projectId)
        {
            return set.Where(x => x.ProjectId == projectId)
                      .OrderByDescending(x => x.ValueId.Count())
                      .ThenByDescending(x => x.ValueId)
                      .FirstOrDefault()
                      .ValueId;
        }

        public static int NextValueId(this DbSet<Task> set, int projectId)
        {
            return Convert.ToInt32(set.LastValueId(projectId)) + 1;
        }
    }
}

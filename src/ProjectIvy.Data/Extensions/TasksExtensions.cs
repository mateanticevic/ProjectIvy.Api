using Microsoft.EntityFrameworkCore;
using ProjectIvy.Model.Database.Main.Org;
using System;
using System.Linq;

namespace ProjectIvy.Data.Extensions
{
    public static class TasksExtensions
    {
        public static string LastValueId(this DbSet<Task> set, int projectId)
        {
            return set.Where(x => x.ProjectId == projectId)
                      .OrderByDescending(x => x.ValueId.Count())
                      .ThenByDescending(x => x.ValueId)
                      .FirstOrDefault()
                      ?.ValueId;
        }

        public static int NextValueId(this DbSet<Task> set, int projectId)
        {
            string lastValueId = set.LastValueId(projectId);

            lastValueId = string.IsNullOrEmpty(lastValueId) ? 0.ToString() : lastValueId;

            return Convert.ToInt32(lastValueId) + 1;
        }
    }
}

using Microsoft.EntityFrameworkCore;
using ProjectIvy.Data.Extensions;
using ProjectIvy.Model.Database.Main.Org;
using System;
using System.Linq;

namespace ProjectIvy.Business.MapExtensions
{
    public static class ToDoExtensions
    {
        public static int NextValueId(this DbSet<ToDo> set, int userId)
        {
            string lastValueId = set.LastValueId(userId);

            lastValueId = string.IsNullOrEmpty(lastValueId) ? 0.ToString() : lastValueId;

            return Convert.ToInt32(lastValueId) + 1;
        }

        public static string LastValueId(this DbSet<ToDo> set, int userId)
        {
            return set.WhereUser(userId)
                      .OrderByDescending(x => x.ValueId.Count())
                      .ThenByDescending(x => x.ValueId)
                      .FirstOrDefault()
                      ?.ValueId;
        }
    }
}

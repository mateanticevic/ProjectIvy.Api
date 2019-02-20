using ProjectIvy.Data.DbContexts;
using ProjectIvy.Data.Extensions;
using ProjectIvy.Model.Binding.Task;
using ProjectIvy.Model.Database.Main.Org;
using System.Linq;

namespace ProjectIvy.Business.MapExtensions
{
    public static class TaskExtensions
    {
        public static Task ToEntity(this TaskBinding binding, MainContext context, Task entity = null)
        {
            entity = entity == null ? new Task() : entity;

            entity.ProjectId = context.Projects.SingleOrDefault(x => x.ValueId == binding.ProjectId).Id;
            entity.Description = binding.Description;
            entity.Name = binding.Name;
            entity.TaskTypeId = context.TaskTypes.SingleOrDefault(x => x.ValueId == binding.TypeId).Id;

            return entity;
        }
    }
}

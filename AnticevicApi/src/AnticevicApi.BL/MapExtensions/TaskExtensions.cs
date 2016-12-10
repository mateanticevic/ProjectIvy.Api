using AnticevicApi.DL.DbContexts;
using AnticevicApi.DL.Extensions;
using AnticevicApi.DL.Helpers;
using AnticevicApi.Model.Binding.Task;
using AnticevicApi.Model.Constants.Database;
using AnticevicApi.Model.Database.Main.Org;
using System.Linq;

namespace AnticevicApi.BL.MapExtensions
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

using AnticevicApi.DL.Helpers;
using AnticevicApi.Model.Binding.Task;
using AnticevicApi.Model.Constants.Database;
using AnticevicApi.Model.Database.Main.Org;

namespace AnticevicApi.BL.MapExtensions
{
    public static class TaskExtensions
    {
        public static Task ToEntity(this TaskBinding binding, Task entity = null)
        {
            entity = entity == null ? new Task() : entity;

            entity.ProjectId = ProjectHelper.GetId(binding.ProjectId);
            entity.Description = binding.Description;
            entity.Name = binding.Name;
            entity.TaskTypeId = TaskTypes.GetId(binding.TypeId);

            return entity;
        }
    }
}

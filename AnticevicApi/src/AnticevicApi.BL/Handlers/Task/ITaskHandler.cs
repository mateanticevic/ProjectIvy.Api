using AnticevicApi.Model.Binding.Task;
using System.Collections.Generic;
using View = AnticevicApi.Model.View.Task;

namespace AnticevicApi.BL.Handlers.Task
{
    public interface ITaskHandler : IHandler
    {
        string Create(TaskBinding binding);

        void CreateChange(TaskChangeBinding binding);

        void Delete(string projectValueId, string taskValueId);

        IEnumerable<View.Task> Get(string projectValueId);

        View.Task Get(string projectValueId, string taskValueId);

        IEnumerable<View.Task> Get(TaskGetBinding binding);

        IEnumerable<View.Priority> GetPriorities();

        IEnumerable<View.Status> GetStatuses();

        IEnumerable<View.Type> GetTypes();

    }
}

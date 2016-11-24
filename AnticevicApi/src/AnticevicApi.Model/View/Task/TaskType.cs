using DatabaseModel = AnticevicApi.Model.Database.Main;

namespace AnticevicApi.Model.View.Task
{
    public class TaskType
    {
        public TaskType(DatabaseModel.Org.TaskType x)
        {
            Name = x.Name;
            ValueId = x.ValueId;
        }

        public string Name { get; set; }
        public string ValueId { get; set; }
    }
}

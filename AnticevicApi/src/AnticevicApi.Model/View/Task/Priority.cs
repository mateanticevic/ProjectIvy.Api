using DatabaseModel = AnticevicApi.Model.Database.Main;

namespace AnticevicApi.Model.View.Task
{
    public class Priority
    {
        public Priority(DatabaseModel.Org.TaskPriority x)
        {
            Name = x.Name;
            ValueId = x.ValueId;
        }

        public string Name { get; set; }

        public string ValueId { get; set; }
    }
}

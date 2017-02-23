using DatabaseModel = AnticevicApi.Model.Database.Main;

namespace AnticevicApi.Model.View.Task
{
    public class Status
    {
        public Status(DatabaseModel.Org.TaskStatus x)
        {
            Name = x.Name;
            ValueId = x.ValueId;
        }

        public string Name { get; set; }

        public string ValueId { get; set; }
    }
}

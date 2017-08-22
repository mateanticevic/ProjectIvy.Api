using DatabaseModel = AnticevicApi.Model.Database.Main;

namespace AnticevicApi.Model.View.Task
{
    public class Type
    {
        public Type(DatabaseModel.Org.TaskType x)
        {
            Name = x.Name;
            Id = x.ValueId;
        }

        public string Id { get; set; }

        public string Name { get; set; }
    }
}

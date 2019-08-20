using DatabaseModel = ProjectIvy.Model.Database.Main;

namespace ProjectIvy.Model.View.ToDo
{
    public class ToDo
    {
        public ToDo(DatabaseModel.Org.ToDo t)
        {
            Id = t.ValueId;
            IsDone = t.IsDone;
            Name = t.Name;
        }

        public string Id { get; set; }

        public bool IsDone { get; set; }

        public string Name { get; set; }
    }
}

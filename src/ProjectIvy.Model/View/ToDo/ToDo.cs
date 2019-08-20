using DatabaseModel = ProjectIvy.Model.Database.Main;

namespace ProjectIvy.Model.View.ToDo
{
    public class ToDo
    {
        public ToDo(DatabaseModel.Org.ToDo t)
        {
            IsDone = t.IsDone;
            Name = t.Name;
        }

        public bool IsDone { get; set; }

        public string Name { get; set; }
    }
}

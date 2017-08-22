using DatabaseModel = AnticevicApi.Model.Database.Main;

namespace AnticevicApi.Model.View.Project
{
    public class Project
    {
        public Project(DatabaseModel.Org.Project x)
        {
            Id = x.ValueId;
            Name = x.Name;
        }

        public string Id { get; set; }

        public string Name { get; set; }
    }
}

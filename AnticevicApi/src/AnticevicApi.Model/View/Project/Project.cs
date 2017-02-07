using DatabaseModel = AnticevicApi.Model.Database.Main;

namespace AnticevicApi.Model.View.Project
{
    public class Project
    {
        public Project(DatabaseModel.Org.Project x)
        {
            ValueId = x.ValueId;
            Name = x.Name;
        }

        public string Name { get; set; }

        public string ValueId { get; set; }
    }
}

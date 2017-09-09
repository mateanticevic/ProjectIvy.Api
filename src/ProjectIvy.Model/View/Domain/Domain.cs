using DatabaseModel = ProjectIvy.Model.Database.Main;

namespace ProjectIvy.Model.View.Domain
{
    public class Domain
    {
        public Domain(DatabaseModel.Net.Domain domain)
        {
            Id = domain.ValueId;
        }

        public string Id { get; set; }
    }
}

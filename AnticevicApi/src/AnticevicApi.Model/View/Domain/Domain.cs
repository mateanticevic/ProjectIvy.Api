using DatabaseModel = AnticevicApi.Model.Database.Main;

namespace AnticevicApi.Model.View.Domain
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

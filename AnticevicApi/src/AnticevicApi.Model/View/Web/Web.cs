using DatabaseModel = AnticevicApi.Model.Database.Main;

namespace AnticevicApi.Model.View.Web
{
    public class Web
    {
        public Web(DatabaseModel.Net.Web web)
        {
            Id = web.ValueId;
            Name = web.Name;
        }

        public string Id { get; set; }

        public string Name { get; set; }
    }
}

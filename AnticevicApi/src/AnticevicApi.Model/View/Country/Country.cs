using DatabaseModel = AnticevicApi.Model.Database.Main;

namespace AnticevicApi.Model.View.Country
{
    public class Country
    {
        public Country(DatabaseModel.Common.Country x)
        {
            Name = x.Name;
            Id = x.ValueId;
        }

        public string Id { get; set; }

        public string Name { get; set; }
    }
}

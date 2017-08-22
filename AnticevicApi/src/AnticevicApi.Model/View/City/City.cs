using DatabaseModel = AnticevicApi.Model.Database.Main;

namespace AnticevicApi.Model.View.City
{
    public class City
    {
        public City(DatabaseModel.Common.City x)
        {
            Name = x.Name;
            Id = x.ValueId;
        }

        public string Id { get; set; }

        public string Name { get; set; }
    }
}

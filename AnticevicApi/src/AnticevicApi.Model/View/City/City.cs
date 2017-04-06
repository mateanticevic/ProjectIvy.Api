using DatabaseModel = AnticevicApi.Model.Database.Main;

namespace AnticevicApi.Model.View.City
{
    public class City
    {
        public City(DatabaseModel.Common.City x)
        {
            Name = x.Name;
            ValueId = x.ValueId;
        }

        public string ValueId { get; set; }

        public string Name { get; set; }
    }
}

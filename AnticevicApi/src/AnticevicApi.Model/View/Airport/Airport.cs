using DatabaseModel = AnticevicApi.Model.Database.Main;

namespace AnticevicApi.Model.View.Airport
{
    public class Airport
    {
        public Airport(DatabaseModel.Transport.Airport x)
        {
            IATA = x.IATA;
            Name = x.Name;
        }

        public string IATA { get; set; }

        public string Name { get; set; }
    }
}

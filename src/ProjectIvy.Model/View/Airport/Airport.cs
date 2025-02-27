using ProjectIvy.Common.Extensions;
using DatabaseModel = ProjectIvy.Model.Database.Main;

namespace ProjectIvy.Model.View.Airport;

public class Airport
{
    public Airport() { }

    public Airport(DatabaseModel.Transport.Airport x)
    {
        Iata = x.Iata;
        Name = x.Name;
        Poi = x.Poi?.ConvertTo(y => new Poi.Poi(y));
    }

    public Poi.Poi Poi { get; set; }

    public string Iata { get; set; }

    public string Name { get; set; }
}

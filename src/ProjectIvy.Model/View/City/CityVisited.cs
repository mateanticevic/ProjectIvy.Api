using Newtonsoft.Json;
using DatabaseModel = ProjectIvy.Model.Database.Main;

namespace ProjectIvy.Model.View.City;

public class CityVisited : City
{
    public CityVisited(DatabaseModel.Common.City city, DateTime? enterTime, DateTime? exitTime) : base(city)
    {
        EnterTime = enterTime;
        ExitTime = exitTime;
    }

    [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
    public DateTime? EnterTime { get; set; }

    [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
    public DateTime? ExitTime { get; set; }
}

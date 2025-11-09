using System.Linq;
using Newtonsoft.Json;

namespace ProjectIvy.Model.View.Location;

public class Location
{
	public Location(Database.Main.Tracking.Location l)
	{
		Id = l.ValueId;
		Name = l.Name;
		Geohashes = l.Geohashes?.Select(x => x.Geohash).ToList();
		Type = new LocationType(l.LocationType);
	}

	public string Id { get; set; }

	public string Name { get; set; }

	[JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
	public IEnumerable<string> Geohashes { get; set; }

	public LocationType Type { get; set; }
}

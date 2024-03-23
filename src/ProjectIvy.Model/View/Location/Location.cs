using System.Linq;
using System.Text.Json.Serialization;
using Newtonsoft.Json;

namespace ProjectIvy.Model.View.Location
{
	public class Location
	{
		public Location(Database.Main.Tracking.Location l)
		{
			Id = l.ValueId;
			Name = l.Name;
			Geohashes = l.Geohashes?.Select(x => x.Geohash).ToList();
		}

		public string Id { get; set; }

		public string Name { get; set; }

		[JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
		public IEnumerable<string> Geohashes { get; set; }
	}
}


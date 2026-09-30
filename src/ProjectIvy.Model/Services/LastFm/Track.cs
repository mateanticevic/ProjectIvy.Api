using Newtonsoft.Json;
using System.Collections.Generic;

namespace ProjectIvy.Model.Services.LastFm;

public class Track
{
    [JsonProperty("artist")]
    public Artist Artist { get; set; }

    [JsonProperty("date")]
    public Timestamp Date { get; set; }

    [JsonProperty("duration")]
    public int Duration { get; set; }

    [JsonProperty("image")]
    public IEnumerable<Image> Images { get; set; }

    [JsonProperty("name")]
    public string Name { get; set; }

    [JsonProperty("playcount")]
    public int PlayCount { get; set; }
}

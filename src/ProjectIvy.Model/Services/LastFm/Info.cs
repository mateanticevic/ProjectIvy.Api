using Newtonsoft.Json;

namespace ProjectIvy.Model.Services.LastFm;

public class Info
{
    [JsonProperty("playcount")]
    public int PlayCount { get; set; }
}

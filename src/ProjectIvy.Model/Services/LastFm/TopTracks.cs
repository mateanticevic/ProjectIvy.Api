using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;

namespace ProjectIvy.Model.Services.LastFm;

public class TopTracks
{
    [JsonProperty("@attr")]
    public string Prop { get; set; }
}

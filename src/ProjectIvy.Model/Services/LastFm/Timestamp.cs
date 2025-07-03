using Newtonsoft.Json;

namespace ProjectIvy.Model.Services.LastFm;

public class Timestamp
{
    [JsonProperty("#text")]
    public string Text { get; set; }

    [JsonProperty("uts")]
    public int Unix { get; set; }
}

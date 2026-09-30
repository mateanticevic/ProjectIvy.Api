using Newtonsoft.Json;

namespace ProjectIvy.Model.Services.LastFm;

public class Image
{
    [JsonProperty("size")]
    public string Size { get; set; }

    [JsonProperty("#text")]
    public string Url { get; set; }
}

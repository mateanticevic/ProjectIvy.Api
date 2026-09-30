using Newtonsoft.Json;

namespace ProjectIvy.Model.Services.LastFm;

public class Artist
{
    public string ArtistName => string.IsNullOrEmpty(Name) ? Text : Name;

    [JsonProperty("@attr")]
    public ArtistAttributes Attributes { get; set; }

    [JsonProperty("name")]
    public string Name { get; set; }

    [JsonProperty("playcount")]
    public string Playcount { get; set; }

    [JsonProperty("#text")]
    public string Text { get; set; }

    [JsonProperty("url")]
    public string Url { get; set; }
}

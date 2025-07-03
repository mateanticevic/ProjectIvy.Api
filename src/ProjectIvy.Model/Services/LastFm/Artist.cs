using Newtonsoft.Json;

namespace ProjectIvy.Model.Services.LastFm;

public class Artist
{
    [JsonProperty("name")]
    public string Name { get; set; }

    [JsonProperty("#text")]
    public string Text { get; set; }

    [JsonProperty("playcount")]
    public string Playcount { get; set; }

    [JsonProperty("url")]
    public string Url { get; set; }

    [JsonProperty("@attr")]
    public ArtistAttributes Attributes { get; set; }

    public string ArtistName => string.IsNullOrEmpty(Name) ? Text : Name;
}

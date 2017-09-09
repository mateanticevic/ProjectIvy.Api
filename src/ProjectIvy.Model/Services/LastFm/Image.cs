using Newtonsoft.Json;

namespace ProjectIvy.Model.Services.LastFm
{
    public class Image
    {
        [JsonProperty("#text")]
        public string Url { get; set; }

        [JsonProperty("size")]
        public string Size { get; set; }
    }
}

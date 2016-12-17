using Newtonsoft.Json;

namespace AnticevicApi.Model.Services.LastFm
{
    public class Image
    {
        [JsonProperty("#text")]
        public string Url { get; set; }

        [JsonProperty("size")]
        public string Size { get; set; }
    }
}

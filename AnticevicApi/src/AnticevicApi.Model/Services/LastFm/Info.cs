using Newtonsoft.Json;

namespace AnticevicApi.Model.Services.LastFm
{
    public class Info
    {
        [JsonProperty("playcount")]
        public int PlayCount { get; set; }
    }
}

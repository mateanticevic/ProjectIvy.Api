using Newtonsoft.Json;

namespace ProjectIvy.Model.Services.LastFm
{
    public class Artist
    {
        [JsonProperty("name")]
        public string Name { get; set; }

        [JsonProperty("#text")]
        public string Text { get; set; }

        public string ArtistName
        {
            get
            {
                return string.IsNullOrEmpty(Name) ? Text : Name;
            }
        }
    }
}

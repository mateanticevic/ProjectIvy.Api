using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace AnticevicApi.Model.Services.LastFm
{
    public class Track
    {
        [JsonProperty("name")]
        public string Name { get; set; }
        [JsonProperty("duration")]
        public int Duration { get; set; }
        [JsonProperty("playcount")]
        public int PlayCount { get; set; }
        [JsonProperty("artist")]
        public Artist Artist { get; set; }
    }
}

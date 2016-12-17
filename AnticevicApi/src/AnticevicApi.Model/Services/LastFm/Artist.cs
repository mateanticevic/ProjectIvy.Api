using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace AnticevicApi.Model.Services.LastFm
{
    public class Artist
    {
        [JsonProperty("name")]
        public string Name { get; set; }
    }
}

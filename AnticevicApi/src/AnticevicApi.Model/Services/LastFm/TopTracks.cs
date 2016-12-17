using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Newtonsoft.Json.Serialization;
using Newtonsoft.Json;

namespace AnticevicApi.Model.Services.LastFm
{
    public class TopTracks
    {
        [JsonProperty("@attr")]
        public string Prop { get; set; }
    }
}

using ProjectIvy.Model.Binding.Common;
using ProjectIvy.Model.Services.LastFm;
using Newtonsoft.Json.Linq;
using System.Collections.Generic;
using System.Net.Http;
using System.Threading.Tasks;

namespace ProjectIvy.DL.Services.LastFm
{
    public class UserHelper : IUserHelper
    {
        private string _url;

        public UserHelper(Common.Configuration.Services.LastFm settings)
        {
            _url = settings.Url.SetKey(settings.Key);
        }

        public async Task<IEnumerable<Track>> GetTopTracks()
        {
            var hc = new HttpClient();

            var response = await hc.GetStringAsync("http://ws.audioscrobbler.com/2.0/?method=user.gettoptracks&user=tema_tracid&api_key=60ace6d94d756d1a7b417bbcd0e42b34&format=json");

            var jo = JObject.Parse(response.Replace("@", string.Empty));

            var tt = jo.SelectToken("toptracks");

            var track = tt.SelectToken("track");

            return track.ToObject<IEnumerable<Track>>();
        }

        public async Task<Info> GetTotalCount(string username)
        {
            var hc = new HttpClient();

            string url = _url.SetMethod(ApiMethod.User.GetInfo)
                             .SetUsername(username);

            var json = await hc.GetStringAsync(url);

            var infoObject = JObject.Parse(json).SelectToken("user");

            return infoObject.ToObject<Info>();
        }

        public async Task<IEnumerable<Track>> GetTracks(string username, FilteredPagedBinding filter)
        {
            var hc = new HttpClient();

            string url = _url.SetMethod(ApiMethod.User.GetRecentTracks)
                             .SetUsername(username)
                             .SetFrom(filter.From)
                             .SetTo(filter.To)
                             .SetPage(filter.Page)
                             .SetPageSize(filter.PageSize);

            var json = await hc.GetStringAsync(url);

            var tracks = JObject.Parse(json).SelectToken("recenttracks")
                                            .SelectToken("track");

            return tracks.ToObject<IEnumerable<Track>>();
        }
    }
}

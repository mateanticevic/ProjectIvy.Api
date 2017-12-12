using ProjectIvy.Model.Binding.Common;
using ProjectIvy.Model.Services.LastFm;
using Newtonsoft.Json.Linq;
using System.Collections.Generic;
using System.Net.Http;
using System.Threading.Tasks;
using ProjectIvy.Common.Extensions;
using ProjectIvy.Model.Binding;
using ProjectIvy.Model.Services.LastFm.Request;

namespace ProjectIvy.DL.Services.LastFm
{
    public class UserHelper : IUserHelper
    {
        private readonly string _url;

        private readonly Common.Configuration.Services.LastFm _settings;

        public UserHelper(Common.Configuration.Services.LastFm settings)
        {
            _url = settings.Url.SetKey(settings.Key);
            _settings = settings;
        }

        public async Task<IEnumerable<Track>> GetLovedTracks(string username)
        {
            using (var client = new HttpClient())
            {
                var request = new UserGetLovedTracks(_settings.Url, _settings.Key, username)
                {
                };

                var json = await client.GetStringAsync(request.ToUrl());

                var tracks = JObject.Parse(json).SelectToken("lovedtracks")
                                                .SelectToken("track");

                return tracks.ToObject<IEnumerable<Track>>();
            }
        }
        
        public async Task<IEnumerable<Artist>> GetTopArtists(string username)
        {
            using (var client = new HttpClient())
            {
                var request = new UserGetTopArtists(_settings.Url, _settings.Key, username)
                {
                };

                var json = await client.GetStringAsync(request.ToUrl());

                var tracks = JObject.Parse(json).SelectToken("topartists")
                                                .SelectToken("artist");

                return tracks.ToObject<IEnumerable<Artist>>();
            }
        }

        public async Task<IEnumerable<Track>> GetTopTracks(string username)
        {
            using (var client = new HttpClient())
            {
                var request = new UserGetTopTracks(_settings.Url, _settings.Key, username)
                {
                    Api_Key = _settings.Key,
                    Period = Period.Overall,
                    User = username
                };

                var json = await client.GetStringAsync(request.ToUrl());

                var tracks = JObject.Parse(json).SelectToken("toptracks")
                                    .SelectToken("track");

                return tracks.ToObject<IEnumerable<Track>>();
            }
        }

        public async Task<Info> GetTotalCount(string username)
        {
            using (var client = new HttpClient())
            {
                var request = new UserGetInfo(_settings.Url, _settings.Key, username);
                 
                var json = await client.GetStringAsync(request.ToUrl());

                var infoObject = JObject.Parse(json).SelectToken("user");

                return infoObject.ToObject<Info>();
            }
        }

        public async Task<IEnumerable<Track>> GetTracks(string username, FilteredPagedBinding filter)
        {
            using (var client = new HttpClient())
            {
                var request = new UserGetRecentTracks(_settings.Url, _settings.Key, username)
                {
                    Api_Key = _settings.Key,
                    From = filter.From?.ToUnix().ToString(),
                    User = username,
                    To = filter.To?.ToUnix().ToString()
                };

                var json = await client.GetStringAsync(request.ToUrl());

                var tracks = JObject.Parse(json).SelectToken("recenttracks")
                                                .SelectToken("track");

                return tracks.ToObject<IEnumerable<Track>>();
            }
        }
    }
}

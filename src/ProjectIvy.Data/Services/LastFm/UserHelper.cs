using System.Collections.Generic;
using System.Net.Http;
using System.Threading.Tasks;
using Newtonsoft.Json.Linq;
using ProjectIvy.Common.Extensions;
using ProjectIvy.Model.Binding;
using ProjectIvy.Model.Services.LastFm;
using ProjectIvy.Model.Services.LastFm.Request;

namespace ProjectIvy.Data.Services.LastFm;

public class UserHelper : IUserHelper
{
    private readonly string _key;
    private readonly string _url = "http://ws.audioscrobbler.com/2.0/?method={method}&user={username}&api_key={key}&format=json";

    public UserHelper(string key)
    {
        _key = key;
    }

    public async Task<IEnumerable<Track>> GetLovedTracks(string username)
    {
        using (var client = new HttpClient())
        {
            var request = new UserGetLovedTracks(_url, _key, username)
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
            var request = new UserGetTopArtists(_url, _key, username)
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
            var request = new UserGetTopTracks(_url, _key, username)
            {
                Api_Key = _key,
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
            var request = new UserGetInfo(_url, _key, username);

            var json = await client.GetStringAsync(request.ToUrl());

            var infoObject = JObject.Parse(json).SelectToken("user");

            return infoObject.ToObject<Info>();
        }
    }

    public async Task<IEnumerable<Track>> GetTracks(string username, FilteredPagedBinding filter)
    {
        using (var client = new HttpClient())
        {
            var request = new UserGetRecentTracks(_url, _key, username)
            {
                Api_Key = _key,
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

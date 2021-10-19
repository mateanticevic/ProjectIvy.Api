using ProjectIvy.Business.Handlers;
using ProjectIvy.Data.Services.LastFm;
using ProjectIvy.Model.Binding;
using ProjectIvy.Model.View.Services.LastFm;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ProjectIvy.Business.Services.LastFm
{
    public class LastFmHandler : Handler<LastFmHandler>, ILastFmHandler
    {
        private readonly IUserHelper _userHelper;

        public LastFmHandler(IHandlerContext<LastFmHandler> context, IUserHelper userHelper) : base(context)
        {
            _userHelper = userHelper;
        }

        public async Task<int> GetTotalCount()
        {
            using (var db = GetMainContext())
            {
                string username = db.Users.SingleOrDefault(x => x.Id == UserId.Value)
                                          .LastFmUsername;

                var info = await _userHelper.GetTotalCount(username);
                return info.PlayCount;
            }
        }

        public async Task<IEnumerable<Track>> GetLovedTracks()
        {
            using (var db = GetMainContext())
            {
                string username = db.Users.SingleOrDefault(x => x.Id == UserId.Value).LastFmUsername;

                var info = await _userHelper.GetLovedTracks(username);
                return info.Select(x => new Track(x));
            }
        }

        public async Task<IEnumerable<Artist>> GetTopArtists()
        {
            using (var db = GetMainContext())
            {
                string username = db.Users.SingleOrDefault(x => x.Id == UserId.Value).LastFmUsername;

                var artists = await _userHelper.GetTopArtists(username);
                return artists.Select(x => new Artist(x));
            }
        }

        public async Task<IEnumerable<Track>> GetTopTracks()
        {
            using (var db = GetMainContext())
            {
                string username = db.Users.SingleOrDefault(x => x.Id == UserId.Value)
                    .LastFmUsername;

                var info = await _userHelper.GetTopTracks(username);
                return info.Select(x => new Track(x));
            }
        }

        public async Task<IEnumerable<Track>> GetTracks(FilteredPagedBinding binding)
        {
            using (var db = GetMainContext())
            {
                string username = db.Users.SingleOrDefault(x => x.Id == UserId.Value)
                                          .LastFmUsername;

                var tracks = await _userHelper.GetTracks(username, binding);

                return tracks.Select(x => new Track(x));
            }
        }
    }
}

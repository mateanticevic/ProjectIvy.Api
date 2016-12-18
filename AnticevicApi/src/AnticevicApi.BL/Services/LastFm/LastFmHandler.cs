using AnticevicApi.BL.Handlers;
using AnticevicApi.DL.Services.LastFm;
using AnticevicApi.Model.Binding.Common;
using AnticevicApi.Model.View.Services.LastFm;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace AnticevicApi.BL.Services.LastFm
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
                string username = db.Users.SingleOrDefault(x => x.Id == User.Id)
                                          .LastFmUsername;

                var info = await _userHelper.GetTotalCount(username);
                return info.PlayCount;
            }
        }

        public async Task<IEnumerable<Track>> GetTracks(FilteredPagedBinding binding)
        {
            using (var db = GetMainContext())
            {
                string username = db.Users.SingleOrDefault(x => x.Id == User.Id)
                                          .LastFmUsername;

                var tracks = await _userHelper.GetTracks(username, binding);

                return tracks.Select(x => new Track(x));
            }
        }
    }
}

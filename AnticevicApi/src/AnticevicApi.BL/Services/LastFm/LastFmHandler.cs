using AnticevicApi.BL.Handlers;
using AnticevicApi.DL.Services.LastFm;
using System.Linq;
using System.Threading.Tasks;

namespace AnticevicApi.BL.Services.LastFm
{
    public class LastFmHandler : Handler, ILastFmHandler
    {
        private readonly IUserHelper _userHelper;

        public LastFmHandler(IUserHelper userHelper)
        {
            _userHelper = userHelper;
        }

        public async Task<int> GetTotalCount()
        {
            using (var db = GetMainContext())
            {
                string username = db.Users.SingleOrDefault(x => x.Id == UserId)
                                          .LastFmUsername;

                var info = await _userHelper.GetTotalCount(username);
                return info.PlayCount;
            }
        }
    }
}

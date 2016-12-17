using AnticevicApi.DL.Services.LastFm;
using AnticevicApi.Extensions.Factory;

namespace AnticevicApi.BL.Services.LastFm
{
    public class LastFmFactory : IServiceFactory<ILastFmHandler>
    {
        private readonly IUserHelper _userHelper;

        public LastFmFactory(IUserHelper userHelper)
        {
            _userHelper = userHelper;
        }

        public ILastFmHandler Build()
        {
            return new LastFmHandler(_userHelper);
        }
    }
}

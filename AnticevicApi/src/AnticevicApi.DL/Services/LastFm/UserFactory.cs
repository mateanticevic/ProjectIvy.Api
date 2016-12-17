using AnticevicApi.Common.Configuration;
using AnticevicApi.Extensions.Factory;
using Microsoft.Extensions.Options;

namespace AnticevicApi.DL.Services.LastFm
{
    public class UserFactory : IServiceFactory<IUserHelper>
    {
        private readonly Common.Configuration.Services.LastFm _lastFmSettings;

        public UserFactory(IOptions<AppSettings> options)
        {
            _lastFmSettings = options.Value.Services.LastFm;
        }

        public IUserHelper Build()
        {
            return new UserHelper(_lastFmSettings);
        }
    }
}

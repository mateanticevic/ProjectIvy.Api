using ProjectIvy.Common.Configuration;
using ProjectIvy.Extensions.Factory;
using Microsoft.Extensions.Options;

namespace ProjectIvy.DL.Services.LastFm
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

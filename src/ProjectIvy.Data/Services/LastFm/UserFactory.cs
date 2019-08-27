using Microsoft.Extensions.Options;
using ProjectIvy.Common.Configuration;
using ProjectIvy.Common.Interfaces;

namespace ProjectIvy.Data.Services.LastFm
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

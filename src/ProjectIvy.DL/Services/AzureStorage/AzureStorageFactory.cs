using Microsoft.Extensions.Options;
using ProjectIvy.Common.Configuration;
using ProjectIvy.Common.Interfaces;

namespace ProjectIvy.DL.Services.AzureStorage
{
    public class AzureStorageFactory : IServiceFactory<IAzureStorageHelper>
    {
        private readonly Common.Configuration.Services.AzureStorage _settings;

        public AzureStorageFactory(IOptions<AppSettings> options)
        {
            _settings = options.Value.Services.AzureStorage;
        }

        public IAzureStorageHelper Build()
        {
            return new AzureStorageHelper(_settings);
        }
    }
}

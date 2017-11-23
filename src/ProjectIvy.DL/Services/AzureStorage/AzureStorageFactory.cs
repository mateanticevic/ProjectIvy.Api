using ProjectIvy.Common.Configuration;
using ProjectIvy.Common.Interfaces;
using Microsoft.Extensions.Options;

namespace ProjectIvy.DL.Services.AzureStorage
{
    public class AzureStorageFactory : IServiceFactory<IAzureStorageHelper>
    {
        private readonly Common.Configuration.Services.AzureStorage _settings;

        public AzureStorageFactory(IOptions<AppSettings> options) => _settings = options.Value.Services.AzureStorage;

        public IAzureStorageHelper Build() => new AzureStorageHelper(_settings);
    }
}

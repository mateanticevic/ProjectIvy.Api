using Microsoft.Extensions.Logging;

namespace AnticevicApi.BL.Handlers
{
    public interface IHandler
    {
        void Initialize(string connectionString, int userId, ILogger logger);

        string ConnectionString { get; set; }
        bool IsInitialized { get; }
        ILogger Logger { get; }
        int UserId { get; set; }
    }
}

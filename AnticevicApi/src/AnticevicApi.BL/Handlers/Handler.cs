using Microsoft.Extensions.Logging;

namespace AnticevicApi.BL.Handlers
{
    public class Handler : IHandler
    {
        public Handler()
        {

        }

        public Handler(string connectionString, int userId)
        {
            ConnectionString = connectionString;
            UserId = userId;
        }

        public void Initialize(string connectionString, int userId, ILogger logger)
        {
            if(!IsInitialized)
            {
                ConnectionString = connectionString;
                Logger = logger;
                UserId = userId;

                IsInitialized = true;
            }
        }

        public string ConnectionString { get; set; }
        public int UserId { get; set; }
        public bool IsInitialized { get; private set; }
        public ILogger Logger { get; private set; }
    }
}

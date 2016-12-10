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

        public void Initialize(string connectionString, int userId)
        {
            if(!IsInitialized)
            {
                ConnectionString = connectionString;
                UserId = userId;

                IsInitialized = true;
            }
        }

        public string ConnectionString { get; set; }
        public int UserId { get; set; }
        public bool IsInitialized { get; private set; }
    }
}

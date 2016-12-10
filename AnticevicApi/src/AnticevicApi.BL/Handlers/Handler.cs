namespace AnticevicApi.BL.Handlers
{
    public class Handler
    {
        public Handler()
        {

        }

        public Handler(string connectionString, int userId)
        {
            ConnectionString = connectionString;
            UserId = userId;
        }

        protected string ConnectionString { get; set; }
        protected int UserId { get; set; }
    }
}

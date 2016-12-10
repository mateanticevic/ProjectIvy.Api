namespace AnticevicApi.BL.Handlers
{
    public interface IHandler
    {
        void Initialize(string connectionString, int userId);

        string ConnectionString { get; set; }
        bool IsInitialized { get; }
        int UserId { get; set; }
    }
}

namespace AnticevicApi.BL.Handlers
{
    public class Handler
    {
        public Handler()
        {

        }

        public Handler(int userId)
        {
            UserId = userId;
        }

        public int UserId { get; set; }
    }
}

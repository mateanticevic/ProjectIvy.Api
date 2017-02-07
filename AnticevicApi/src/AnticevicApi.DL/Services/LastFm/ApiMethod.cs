namespace AnticevicApi.DL.Services.LastFm
{
    public static class ApiMethod
    {
        public static class User
        {
            public const string GetInfo = Prefix + "getInfo";

            public const string GetRecentTracks = Prefix + "getRecentTracks";

            private const string Prefix = "user.";
        }
    }
}

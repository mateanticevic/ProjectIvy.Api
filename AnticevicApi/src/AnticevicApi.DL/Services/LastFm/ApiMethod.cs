namespace AnticevicApi.DL.Services.LastFm
{
    public static class ApiMethod
    {
        public static class User
        {
            private const string _prefix = "user.";
            public const string GetInfo = _prefix + "getInfo";
            public const string GetRecentTracks = _prefix + "getRecentTracks";
        }
    }
}

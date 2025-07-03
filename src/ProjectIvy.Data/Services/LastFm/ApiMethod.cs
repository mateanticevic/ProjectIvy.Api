namespace ProjectIvy.Data.Services.LastFm;

public static class ApiMethod
{
    public static class User
    {
        public const string GetInfo = Prefix + "getInfo";

        public const string GetRecentTracks = Prefix + "getRecentTracks";

        public const string GetTopArtists = Prefix + "getTopArtists";

        public const string GetTopTracks = Prefix + "getTopTracks";

        private const string Prefix = "user.";
    }
}

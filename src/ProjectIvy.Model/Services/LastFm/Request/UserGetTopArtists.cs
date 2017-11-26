namespace ProjectIvy.Model.Services.LastFm.Request
{
    public class UserGetTopArtists : BaseRequest
    {
        public UserGetTopArtists(string url, string key, string user) : base(url, key, user)
        {
        }

        public string Period { get; set; }

        public override string Method => "user.getTopTracks";
    }
}

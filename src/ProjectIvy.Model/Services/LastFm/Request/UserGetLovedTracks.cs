namespace ProjectIvy.Model.Services.LastFm.Request
{
    public class UserGetLovedTracks : BaseRequest
    {
        public UserGetLovedTracks(string url, string key, string user) : base(url, key, user)
        {
        }

        public override string Method => "user.getLovedTracks";
    }
}

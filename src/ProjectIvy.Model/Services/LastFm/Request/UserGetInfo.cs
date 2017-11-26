namespace ProjectIvy.Model.Services.LastFm.Request
{
    public class UserGetInfo : BaseRequest
    {
        public UserGetInfo(string url, string key, string user) : base(url, key, user)
        {
        }

        public override string Method => "user.getInfo";
    }
}

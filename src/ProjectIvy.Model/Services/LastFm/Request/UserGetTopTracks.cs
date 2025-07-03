namespace ProjectIvy.Model.Services.LastFm.Request;

public class UserGetTopTracks : BaseRequest
{
    public UserGetTopTracks(string url, string key, string user) : base(url, key, user)
    {
    }

    public string Period { get; set; }

    public override string Method => "user.getTopTracks";
}

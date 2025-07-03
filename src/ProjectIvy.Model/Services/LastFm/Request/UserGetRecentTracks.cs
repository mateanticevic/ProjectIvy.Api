namespace ProjectIvy.Model.Services.LastFm.Request;

public class UserGetRecentTracks : BaseRequest
{
    public UserGetRecentTracks(string url, string key, string user) : base(url, key, user)
    {
    }

    public string From { get; set; }

    public string To { get; set; }

    public override string Method => "user.getRecentTracks";
}

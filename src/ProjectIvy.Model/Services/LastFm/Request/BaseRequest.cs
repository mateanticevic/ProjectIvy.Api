using System.Linq;
using System.Web;

namespace ProjectIvy.Model.Services.LastFm.Request
{
    public abstract class BaseRequest
    {
        private readonly string _url;

        protected BaseRequest(string url, string key, string user)
        {
            Api_Key = key;
            User = user;
            _url = url;
        }

        public string Api_Key { get; set; }

        public string User { get; set; }

        public string Format => "json";

        public int? Limit { get; set; }

        public int? Page { get; set; }

        public string ToUrl() => $"{_url}?{ToQueryString()}";

        public string ToQueryString()
        {
            var properties = this.GetType().GetProperties()
                                           .Where(x => x.GetValue(this, null) != null)
                                           .Select(x => x.Name.ToLowerInvariant() + "=" + HttpUtility.UrlEncode(x.GetValue(this, null).ToString()));
                 
            return string.Join("&", properties.ToArray());
        }

        public abstract string Method { get; }
    }
}

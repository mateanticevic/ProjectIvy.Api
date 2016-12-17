namespace AnticevicApi.DL.Services.LastFm
{
    public static class Extensions
    {
        public const string Key = "{key}";
        public const string Method = "{method}";
        public const string Username = "{username}";

        public static string SetKey(this string url, string key)
        {
            return url.Replace(Key, key);
        }

        public static string SetUsername(this string url, string username)
        {
            return url.Replace(Username, username);
        }

        public static string SetMethod(this string url, string method)
        {
            return url.Replace(Method, method);
        }
    }
}

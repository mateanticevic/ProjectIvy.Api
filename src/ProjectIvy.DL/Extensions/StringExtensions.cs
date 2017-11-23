namespace ProjectIvy.DL.Extensions
{
    public static class StringExtensions
    {
        public static string ToValueId(this string name)
        {
            return name.ToLowerInvariant()
                       .Replace(' ', '-');
        }
    }
}

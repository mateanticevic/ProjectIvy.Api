namespace ProjectIvy.Common.Extensions
{
    public static class StringExtensions
    {
        public static object ToProperType(this string value)
        {
            if (int.TryParse(value, out var integer))
                return integer;

            return value;
        }

        public static string NameToValueId(this string name)
        {
            return name.ToLowerInvariant()
                       .Replace(' ', '-');
        }
    }
}

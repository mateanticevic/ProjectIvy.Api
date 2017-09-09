namespace ProjectIvy.Extensions.BuiltInTypes
{
    public static class StringExtensions
    {
        public static object ToProperType(this string value)
        {
            int integer;
            if (int.TryParse(value, out integer))
            {
                return integer;
            }

            return value;
        }

        public static string NameToValueId(this string name)
        {
            return name.ToLowerInvariant()
                       .Replace(' ', '-');
        }
    }
}

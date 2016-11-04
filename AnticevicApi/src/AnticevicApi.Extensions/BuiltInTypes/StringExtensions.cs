namespace AnticevicApi.Extensions.BuiltInTypes
{
    public static class StringExtensions
    {
        public static object ToProperType(this string value)
        {
            int integer;
            if(int.TryParse(value, out integer))
            {
                return integer;
            }

            return value;
        }
    }
}

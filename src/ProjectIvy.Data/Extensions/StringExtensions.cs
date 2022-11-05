using System.Text.RegularExpressions;

namespace ProjectIvy.Data.Extensions
{
    public static class StringExtensions
    {
        private static IEnumerable<(string, string)> _replacements =
            new List<(string, string)>()
            {
                ("#|\'|'°|(|)|:|.|,|-", string.Empty),
                (" ", "-"),
                ("õ|ö|ó|ø", "o"),
                ("á", "a"),
                ("ë|é", "e"),
                ("č|ć", "c"),
                ("ñ", "n"),
                ("š", "s"),
                ("ú", "u"),
                ("ý", "y"),
                ("ž", "z"),
                ("&", "and"),
            };

        public static string ToValueId(this string name)
        {
            string valueId = name.ToLowerInvariant();

            foreach (var replacementRegex in _replacements)
            {
                var regex = new Regex(replacementRegex.Item1);
                valueId = regex.Replace(valueId, replacementRegex.Item2);
            }

            return valueId;
        }
    }
}

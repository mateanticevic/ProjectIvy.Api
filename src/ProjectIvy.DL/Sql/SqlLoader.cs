using System.IO;
using System.Reflection;
using System.Text;

namespace ProjectIvy.DL.Sql
{
    public static class SqlLoader
    {
        public static string Load(string resourceName)
        {
            var stream = typeof(SqlLoader).GetTypeInfo().Assembly.GetManifestResourceStream(resourceName);

            using (var reader = new StreamReader(stream, Encoding.UTF8))
            {
                return reader.ReadToEnd();
            }
        }
    }
}

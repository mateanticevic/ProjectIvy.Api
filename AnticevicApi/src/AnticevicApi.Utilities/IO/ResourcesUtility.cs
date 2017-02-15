using System.IO;
using System.Reflection;
using System.Text;

namespace AnticevicApi.Utilities.IO
{
    public class ResourcesUtility
    {
        public static string Get()
        {
            var assembly = Assembly.GetEntryAssembly();
            var resourceStream = assembly.GetManifestResourceStream("AnticevicApi.deps.json");

            var s = typeof(ResourcesUtility).GetTypeInfo().Assembly.GetManifestResourceNames();

            var x = assembly.GetManifestResourceNames();

            using (var reader = new StreamReader(resourceStream, Encoding.UTF8))
            {
                return reader.ReadToEnd();
            }
        }
    }
}

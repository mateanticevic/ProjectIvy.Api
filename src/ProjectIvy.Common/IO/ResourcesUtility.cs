using System.IO;
using System.Reflection;
using System.Text;

namespace ProjectIvy.Common.IO;

public class ResourcesUtility
{
    public static string Get()
    {
        var assembly = Assembly.GetEntryAssembly();
        var resourceStream = assembly.GetManifestResourceStream("ProjectIvy.deps.json");

        var s = typeof(ResourcesUtility).GetTypeInfo().Assembly.GetManifestResourceNames();

        var x = assembly.GetManifestResourceNames();

        using (var reader = new StreamReader(resourceStream, Encoding.UTF8))
        {
            return reader.ReadToEnd();
        }
    }
}

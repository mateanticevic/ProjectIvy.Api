using AnticevicApi.Utilities.Geo;
using System.IO;
using System.Xml.Linq;
using Xunit;

namespace AnticevicApi.Tests.Utilities
{
    public class KlmHandlerTests
    {
        [Fact]
        public void Import_Simple()
        {
            var rawKlm = File.ReadAllText(@"..\Data\SampleMovement.klm");

            var xDoc = XDocument.Parse(rawKlm);

            var trackings = KmlHandler.ParseKml(xDoc);
        }
    }
}

using ProjectIvy.Utilities.Geo;
using System.IO;
using System.Linq;
using System.Xml.Linq;
using Xunit;

namespace ProjectIvy.Tests.Utilities
{
    public class KlmHandlerTests
    {
        [Fact]
        public void Import_Simple()
        {
            var rawKlm = File.ReadAllText(@"Data\SampleMovement.klm");

            var document = XDocument.Parse(rawKlm);

            Assert.NotNull(document);

            var trackings = KmlHandler.ParseKml(document);

            Assert.True(trackings.Any());
        }
    }
}

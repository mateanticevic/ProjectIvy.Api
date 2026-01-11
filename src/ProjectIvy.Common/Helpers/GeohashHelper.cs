using Geohash;

namespace ProjectIvy.Common.Helpers;

public static class GeohashHelper
{
    public static string LocationToGeohash(double latitude, double longitude, int precision = 7)
    {
        var geohasher = new Geohasher();
        return geohasher.Encode(latitude, longitude, precision);
    }

    public static IEnumerable<string> ResolveChildGeohashes(IEnumerable<string> parentGeohashes, int childPrecision)
    {
        var geohasher = new Geohasher();
        var childGeohashes = new HashSet<string>();

        foreach (string parentGeohash in parentGeohashes)
        {
            if (parentGeohash.Length >= childPrecision)
                childGeohashes.Add(parentGeohash.Substring(0, childPrecision));
            else
            {
                var subhashes = geohasher.GetSubhashes(parentGeohash);
                if (parentGeohash.Length + 1 == childPrecision)
                    foreach (string subhash in subhashes)
                        childGeohashes.Add(subhash);
                else
                    foreach (string subhash in ResolveChildGeohashes(subhashes, childPrecision))
                        childGeohashes.Add(subhash);
            }
        }

        return childGeohashes;
    }
}

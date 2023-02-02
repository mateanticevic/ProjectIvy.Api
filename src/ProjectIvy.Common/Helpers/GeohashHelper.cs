using System;
using Geohash;

namespace ProjectIvy.Common.Helpers
{
	public static class GeohashHelper
	{
        public static string LocationToGeohash(double latitude, double longitude, int precision = 7)
        {
            var geohasher = new Geohasher();
            return geohasher.Encode(latitude, longitude, precision);
        }
    }
}


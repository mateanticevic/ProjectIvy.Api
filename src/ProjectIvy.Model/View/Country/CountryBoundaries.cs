using System.Collections.Generic;

namespace ProjectIvy.Model.View.Country
{
    public class CountryBoundaries
    {
        public Country Country { get; set; }

        public IEnumerable<IEnumerable<Location>> Polygons { get; set; }
    }
}

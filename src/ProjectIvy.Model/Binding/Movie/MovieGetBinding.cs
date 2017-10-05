using ProjectIvy.Model.Binding.Common;
using System.Collections.Generic;

namespace ProjectIvy.Model.Binding.Movie
{
    public class MovieGetBinding : FilteredPagedBinding
    {
        public decimal? RatingHigher { get; set; }

        public decimal? RatingLower { get; set; }

        public short? RuntimeLonger { get; set; }

        public short? RuntimeShorter { get; set; }

        public MovieSort OrderBy { get; set; }

        public string Title { get; set; }

        public IEnumerable<short> MyRating { get; set; }

        public IEnumerable<short> Year { get; set; }
    }
}

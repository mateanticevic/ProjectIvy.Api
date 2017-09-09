using ProjectIvy.Model.Binding.Common;
using System.Collections.Generic;
using System;

namespace ProjectIvy.Model.Binding.Movie
{
    public class MovieGetBinding : FilteredPagedBinding
    {
        public decimal? RatingHigher { get; set; }

        public decimal? RatingLower { get; set; }

        public short? RuntimeLonger { get; set; }

        public short? RuntimeShorter { get; set; }

        public string Sort { get; set; }

        public MovieSort SortBy
        {
            get
            {
                try
                {
                    return (MovieSort)Enum.Parse(typeof(MovieSort), Sort);
                }
                catch
                {
                    return MovieSort.Watched;
                }
            }
        }

        public string Title { get; set; }

        public IEnumerable<short> MyRating { get; set; }

        public IEnumerable<short> Year { get; set; }
    }
}

using AnticevicApi.Common.Extensions;
using AnticevicApi.Model.Binding.Movie;
using AnticevicApi.Model.Database.Main.User;
using System.Linq;

namespace AnticevicApi.DL.Extensions.Entities
{
    public static class MovieExtensions
    {
        public static IQueryable<Movie> Where(this IQueryable<Movie> movies, MovieGetBinding binding)
        {
            return movies.WhereTimestampInclusive(binding)
                         .WhereIf(binding.RatingHigher.HasValue, x => x.Rating > binding.RatingHigher.Value)
                         .WhereIf(binding.RatingLower.HasValue, x => x.Rating < binding.RatingLower.Value)
                         .WhereIf(binding.RuntimeLonger.HasValue, x => x.Runtime > binding.RuntimeLonger.Value)
                         .WhereIf(binding.RuntimeShorter.HasValue, x => x.Runtime < binding.RuntimeShorter.Value)
                         .WhereIf(!string.IsNullOrEmpty(binding.Title), x => x.Title.Contains(binding.Title))
                         .WhereIf(!binding.MyRating.IsNullOrEmpty(), x => binding.MyRating.Contains(x.MyRating))
                         .WhereIf(!binding.Year.IsNullOrEmpty(), x => binding.Year.Contains(x.Year));
        }

        public static IOrderedQueryable<Movie> OrderBy(this IQueryable<Movie> movies, MovieGetBinding binding)
        {
            if (binding.SortBy == MovieSort.MyRating)
            {
                return movies.OrderBy(binding.OrderAscending, x => x.MyRating);
            }
            else if (binding.SortBy == MovieSort.MyRatingDifference)
            {
                return movies.OrderBy(binding.OrderAscending, x => x.MyRating - x.Rating);
            }
            else if (binding.SortBy == MovieSort.Year)
            {
                return movies.OrderBy(binding.OrderAscending, x => x.Year);
            }
            else if (binding.SortBy == MovieSort.Title)
            {
                return movies.OrderBy(binding.OrderAscending, x => x.Title);
            }
            else if (binding.SortBy == MovieSort.Rating)
            {
                return movies.OrderBy(binding.OrderAscending, x => x.Rating);
            }
            else if (binding.SortBy == MovieSort.Runtime)
            {
                return movies.OrderBy(binding.OrderAscending, x => x.Runtime);
            }
            else
            {
                return movies.OrderBy(binding.OrderAscending, x => x.Timestamp);
            }
        }
    }
}

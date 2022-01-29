using Microsoft.EntityFrameworkCore;
using ProjectIvy.Common.Extensions;
using ProjectIvy.Model.Binding.Movie;
using ProjectIvy.Model.Database.Main.User;
using System;
using System.Linq;

namespace ProjectIvy.Data.Extensions.Entities
{
    public static class MovieExtensions
    {
        public static readonly DateTime FirstSunday = new DateTime(2000, 1, 2);

        public static IQueryable<Movie> Where(this IQueryable<Movie> movies, MovieGetBinding binding)
        {
            var days = binding.Day?.Select(x => (int)x).ToList();

            return movies.WhereTimestampInclusive(binding)
                         .WhereIf(binding.RatingHigher.HasValue, x => x.Rating > binding.RatingHigher.Value)
                         .WhereIf(binding.RatingLower.HasValue, x => x.Rating < binding.RatingLower.Value)
                         .WhereIf(binding.RuntimeLonger.HasValue, x => x.Runtime > binding.RuntimeLonger.Value)
                         .WhereIf(binding.RuntimeShorter.HasValue, x => x.Runtime < binding.RuntimeShorter.Value)
                         .WhereIf(!string.IsNullOrEmpty(binding.Title), x => x.Title.Contains(binding.Title))
                         .WhereIf(!binding.MyRating.IsNullOrEmpty(), x => binding.MyRating.Contains(x.MyRating))
                         .WhereIf(!binding.Year.IsNullOrEmpty(), x => binding.Year.Contains(x.Year))
                         .WhereIf(!binding.YearWatched.IsNullOrEmpty(), x => binding.YearWatched.Contains(x.Timestamp.Year))
                         .WhereIf(days, x => days.Contains(((int)EF.Functions.DateDiffDay((DateTime?)FirstSunday, (DateTime?)x.Timestamp)) % 7));
        }

        public static IOrderedQueryable<Movie> OrderBy(this IQueryable<Movie> movies, MovieGetBinding binding)
        {
            if (binding.OrderBy == MovieSort.MyRating)
            {
                return movies.OrderBy(binding.OrderAscending, x => x.MyRating);
            }
            else if (binding.OrderBy == MovieSort.MyRatingDifference)
            {
                return movies.OrderBy(binding.OrderAscending, x => x.MyRating - x.Rating);
            }
            else if (binding.OrderBy == MovieSort.Year)
            {
                return movies.OrderBy(binding.OrderAscending, x => x.Year);
            }
            else if (binding.OrderBy == MovieSort.Title)
            {
                return movies.OrderBy(binding.OrderAscending, x => x.Title);
            }
            else if (binding.OrderBy == MovieSort.Rating)
            {
                return movies.OrderBy(binding.OrderAscending, x => x.Rating);
            }
            else if (binding.OrderBy == MovieSort.Runtime)
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

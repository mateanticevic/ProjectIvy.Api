using Microsoft.EntityFrameworkCore;
using ProjectIvy.Common.Extensions;
using ProjectIvy.Data.Extensions;
using ProjectIvy.Data.Extensions.Entities;
using ProjectIvy.Model.Binding.Movie;
using ProjectIvy.Model.View;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using View = ProjectIvy.Model.View.Movie;

namespace ProjectIvy.Business.Handlers.Movie
{
    public class MovieHandler : Handler<MovieHandler>, IMovieHandler
    {
        public static readonly DateTime FirstSunday = new DateTime(2000, 1, 2);

        public MovieHandler(IHandlerContext<MovieHandler> context) : base(context)
        {
        }

        public PagedView<View.Movie> Get(MovieGetBinding binding)
        {
            using (var db = GetMainContext())
            {
                return db.Movies.WhereUser(User.Id)
                                .Where(binding)
                                .OrderBy(binding)
                                .Select(x => new View.Movie(x))
                                .ToPagedView(binding);
            }
        }

        public View.Movie Get(string imdbId)
        {
            using (var context = GetMainContext())
            {
                return context.Movies.WhereUser(User)
                                     .SingleOrDefault(x => x.ImdbId == imdbId)
                                     .ConvertTo(x => new View.Movie(x));
            }
        }

        public int Count(MovieGetBinding binding)
        {
            using (var db = GetMainContext())
            {
                return db.Movies.WhereUser(User.Id)
                                .Where(binding)
                                .Count();
            }
        }

        public IEnumerable<KeyValuePair<int, int>> CountByDayOfWeek(MovieGetBinding binding)
        {
            using (var db = GetMainContext())
            {
                return db.Movies.WhereUser(User.Id)
                                .Where(binding)
                                .GroupBy(x => ((int)EF.Functions.DateDiffDay((DateTime?)FirstSunday, (DateTime?)x.Timestamp) - 1) % 7)
                                .OrderBy(x => x.Key)
                                .Select(x => new KeyValuePair<int, int>(x.Key, x.Count()))
                                .ToList();
            }
        }

        public IEnumerable<KeyValuePair<int, int>> CountByMonth(MovieGetBinding binding)
        {
            using (var db = GetMainContext())
            {
                return db.Movies.WhereUser(User.Id)
                                .Where(binding)
                                .GroupBy(x => x.Timestamp.Month)
                                .OrderBy(x => x.Key)
                                .Select(x => new KeyValuePair<int, int>(x.Key, x.Count()))
                                .ToList();
            }
        }

        public IEnumerable<GroupedByMonth<int>> CountByMonthOfYear(MovieGetBinding binding)
        {
            using (var db = GetMainContext())
            {
                return db.Movies.WhereUser(User.Id)
                                .Where(binding)
                                .GroupBy(x => new { x.Timestamp.Year, x.Timestamp.Month })
                                .OrderBy(x => x.Key.Year)
                                .ThenBy(x => x.Key.Month)
                                .Select(x => new GroupedByMonth<int>(x.Count(), x.Key.Year, x.Key.Month))
                                .ToList();
            }
        }

        public IEnumerable<KeyValuePair<string, int>> CountByMovieDecade(MovieGetBinding binding)
        {
            using (var db = GetMainContext())
            {
                return db.Movies.WhereUser(User.Id)
                                .Where(binding)
                                .GroupBy(x => x.Year - x.Year % 10)
                                .OrderBy(x => x.Key)
                                .Select(x => new KeyValuePair<string, int>($"{x.Key}'s", x.Count()))
                                .ToList();
            }
        }

        public IEnumerable<KeyValuePair<short, int>> CountByMovieYear(MovieGetBinding binding)
        {
            using (var db = GetMainContext())
            {
                return db.Movies.WhereUser(User.Id)
                                .Where(binding)
                                .GroupBy(x => x.Year)
                                .OrderBy(x => x.Key)
                                .Select(x => new KeyValuePair<short, int>(x.Key, x.Count()))
                                .ToList();
            }
        }

        public IEnumerable<KeyValuePair<short, int>> CountByMyRating(MovieGetBinding binding)
        {
            using (var db = GetMainContext())
            {
                return db.Movies.WhereUser(User.Id)
                                .Where(binding)
                                .GroupBy(x => x.MyRating)
                                .OrderBy(x => x.Key)
                                .Select(x => new KeyValuePair<short, int>(x.Key, x.Count()))
                                .ToList();
            }
        }

        public IEnumerable<KeyValuePair<string, int>> CountByRuntime(MovieGetBinding binding)
        {
            using (var db = GetMainContext())
            {
                return db.Movies.WhereUser(User.Id)
                                .Where(binding)
                                .GroupBy(x => x.Runtime - (x.Runtime % 10) + 10)
                                .OrderBy(x => x.Key)
                                .Select(x => new KeyValuePair<string, int>($"~{x.Key}min", x.Count()))
                                .ToList();
            }
        }

        public IEnumerable<GroupedByYear<int>> CountByYear(MovieGetBinding binding)
        {
            using (var db = GetMainContext())
            {
                return db.Movies.WhereUser(User.Id)
                                .Where(binding)
                                .GroupBy(x => x.Timestamp.Year)
                                .OrderBy(x => x.Key)
                                .Select(x => new GroupedByYear<int>(x.Count(), x.Key))
                                .ToList();
            }
        }

        public double GetMyRatingAverage(MovieGetBinding binding)
        {
            using (var db = GetMainContext())
            {
                var userMovies = db.Movies.WhereUser(User.Id)
                                          .Where(binding);

                double average = (double)userMovies.Sum(x => x.MyRating) / userMovies.Count();

                return Math.Round(average, 1);
            }
        }

        public async Task<IEnumerable<KeyValuePair<int, decimal>>> GetMyRatingAverageByYear(MovieGetBinding binding)
        {
            using (var db = GetMainContext())
            {
                return await db.Movies.WhereUser(User)
                                      .Where(binding)
                                      .GroupBy(x => x.Timestamp.Year)
                                      .OrderBy(x => x.Key)
                                      .Select(x => new KeyValuePair<int, decimal>(x.Key, (decimal)Math.Round(x.Average(y => y.MyRating), 1)))
                                      .ToListAsync();
            }
        }

        public double GetRatingAverage(MovieGetBinding binding)
        {
            using (var db = GetMainContext())
            {
                var userMovies = db.Movies.WhereUser(User.Id)
                                          .Where(binding);

                return Math.Round((double)userMovies.Average(x => x.Rating), 1);
            }
        }

        public async Task<IEnumerable<KeyValuePair<int, decimal>>> GetRatingAverageByYear(MovieGetBinding binding)
        {
            using (var db = GetMainContext())
            {
                return await db.Movies.WhereUser(User)
                                      .Where(binding)
                                      .GroupBy(x => x.Timestamp.Year)
                                      .OrderBy(x => x.Key)
                                      .Select(x => new KeyValuePair<int, decimal>(x.Key, Math.Round(x.Average(y => y.Rating), 1)))
                                      .ToListAsync();
            }
        }

        public int GetRuntimeAverage(MovieGetBinding binding)
        {
            using (var db = GetMainContext())
            {
                return (int) db.Movies.WhereUser(User.Id)
                                      .Where(binding)
                                      .Average(x => x.Runtime);
            }
        }

        public int GetSum(MovieGetBinding binding, Func<Model.Database.Main.User.Movie, int> selector)
        {
            using (var db = GetMainContext())
            {
                return db.Movies.WhereUser(User.Id)
                                .Where(binding)
                                .Sum(selector);
            }
        }
    }
}

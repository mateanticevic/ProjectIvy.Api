using AnticevicApi.DL.Extensions;
using AnticevicApi.DL.Extensions.Entities;
using AnticevicApi.Model.Binding.Movie;
using AnticevicApi.Model.View;
using System.Linq;
using View = AnticevicApi.Model.View.Movie;
using System;

namespace AnticevicApi.BL.Handlers.Movie
{
    public class MovieHandler : Handler<MovieHandler>, IMovieHandler
    {
        public MovieHandler(IHandlerContext<MovieHandler> context) : base(context)
        {
        }

        public PagedView<View.Movie> Get(MovieGetBinding binding)
        {
            using (var db = GetMainContext())
            {
                var query = db.Movies.WhereUser(User.Id)
                                     .Where(binding);

                var items = query.OrderBy(binding)
                                 .Page(binding.ToPagedBinding())
                                 .ToList()
                                 .Select(x => new View.Movie(x));

                int count = query.Count();

                return new PagedView<View.Movie>(items, count);
            }
        }

        public int GetCount(MovieGetBinding binding)
        {
            using (var db = GetMainContext())
            {
                var userMovies = db.Movies.WhereUser(User.Id)
                                          .Where(binding);

                return userMovies.Count();
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

        public double GetRatingAverage(MovieGetBinding binding)
        {
            using (var db = GetMainContext())
            {
                var userMovies = db.Movies.WhereUser(User.Id)
                                          .Where(binding);

                return Math.Round((double)userMovies.Average(x => x.Rating), 1);
            }
        }

        public int GetRuntimeAverage(MovieGetBinding binding)
        {
            using (var db = GetMainContext())
            {
                var userMovies = db.Movies.WhereUser(User.Id)
                                          .Where(binding);

                return (int)userMovies.Average(x => x.Runtime);
            }
        }

        public int GetSum(MovieGetBinding binding, Func<Model.Database.Main.User.Movie, int> selector)
        {
            using (var db = GetMainContext())
            {
                var userMovies = db.Movies.WhereUser(User.Id)
                                          .Where(binding);

                return userMovies.Sum(selector);
            }
        }
    }
}

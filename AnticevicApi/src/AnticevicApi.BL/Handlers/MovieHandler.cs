using AnticevicApi.DL.DbContexts;
using AnticevicApi.DL.Extensions;
using AnticevicApi.Model.Binding.Common;
using AnticevicApi.Model.View.Movie;
using System.Collections.Generic;
using System.Linq;

namespace AnticevicApi.BL.Handlers
{
    public class MovieHandler : Handler
    {
        public MovieHandler(string connectionString, int userId) : base(connectionString, userId)
        {

        }

        public IEnumerable<Movie> Get(FilteredPagedBinding binding)
        {
            using (var db = new MainContext(ConnectionString))
            {
                var movies = db.Movies.WhereUser(UserId)
                                      .WhereTimestampInclusive(binding)
                                      .OrderByDescending(x => x.Timestamp)
                                      .Page(binding.ToPagedBinding());

                return movies.ToList()
                             .Select(x => new Movie(x));
            }
        }

        public int GetCount(FilteredBinding binding)
        {
            using (var db = new MainContext(ConnectionString))
            {
                var userMovies = db.Movies.WhereUser(UserId)
                                          .WhereTimestampInclusive(binding);

                return userMovies.Count();
            }
        }
    }
}

using AnticevicApi.DL.DbContexts;
using AnticevicApi.DL.Extensions;
using AnticevicApi.Model.Binding.Common;
using System.Collections.Generic;
using System.Linq;
using View = AnticevicApi.Model.View.Movie;

namespace AnticevicApi.BL.Handlers.Movie
{
    public class MovieHandler : Handler, IMovieHandler
    {
        public IEnumerable<View.Movie> Get(FilteredPagedBinding binding)
        {
            using (var db = new MainContext(ConnectionString))
            {
                var movies = db.Movies.WhereUser(UserId)
                                      .WhereTimestampInclusive(binding)
                                      .OrderByDescending(x => x.Timestamp)
                                      .Page(binding.ToPagedBinding());

                return movies.ToList()
                             .Select(x => new View.Movie(x));
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

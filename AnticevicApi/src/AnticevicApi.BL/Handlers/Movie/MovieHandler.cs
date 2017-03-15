using AnticevicApi.DL.Extensions;
using AnticevicApi.Model.Binding.Common;
using AnticevicApi.Model.Constants;
using Microsoft.Extensions.Logging;
using System.Collections.Generic;
using System.Linq;
using System;
using View = AnticevicApi.Model.View.Movie;
using AnticevicApi.Model.Binding.Movie;
using AnticevicApi.Common.Extensions;

namespace AnticevicApi.BL.Handlers.Movie
{
    public class MovieHandler : Handler<MovieHandler>, IMovieHandler
    {
        public MovieHandler(IHandlerContext<MovieHandler> context) : base(context)
        {
        }

        public IEnumerable<View.Movie> Get(MovieGetBinding binding)
        {
            using (var db = GetMainContext())
            {
                var movies = db.Movies.WhereUser(User.Id)
                                      .WhereTimestampInclusive(binding)
                                      .WhereIf(binding.RatingHigher.HasValue, x => x.Rating > binding.RatingHigher.Value)
                                      .WhereIf(binding.RatingLower.HasValue, x => x.Rating < binding.RatingLower.Value)
                                      .WhereIf(binding.RuntimeLonger.HasValue, x => x.Runtime > binding.RuntimeLonger.Value)
                                      .WhereIf(binding.RuntimeShorter.HasValue, x => x.Runtime < binding.RuntimeShorter.Value)
                                      .WhereIf(!string.IsNullOrEmpty(binding.Title), x => x.Title.Contains(binding.Title))
                                      .WhereIf(!binding.MyRating.IsNullOrEmpty(), x => binding.MyRating.Contains(x.MyRating))
                                      .WhereIf(!binding.Year.IsNullOrEmpty(), x => binding.Year.Contains(x.Year))
                                      .OrderByDescending(x => x.Timestamp)
                                      .Page(binding.ToPagedBinding());

                return movies.ToList()
                             .Select(x => new View.Movie(x));
            }
        }

        public int GetCount(FilteredBinding binding)
        {
            try
            {
                using (var db = GetMainContext())
                {
                    var userMovies = db.Movies.WhereUser(User.Id)
                                              .WhereTimestampInclusive(binding);

                    return userMovies.Count();
                }
            }
            catch (Exception e)
            {
                Logger.LogError((int)LogEvent.Exception, e, LogMessage.UnknownException);

                throw;
            }
        }
    }
}

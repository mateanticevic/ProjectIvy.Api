using AnticevicApi.Model.Binding.Common;
using AnticevicApi.Model.Binding.Movie;
using System.Collections.Generic;
using View = AnticevicApi.Model.View.Movie;

namespace AnticevicApi.BL.Handlers.Movie
{
    public interface IMovieHandler : IHandler
    {
        IEnumerable<View.Movie> Get(MovieGetBinding binding);

        int GetCount(FilteredBinding binding);
    }
}

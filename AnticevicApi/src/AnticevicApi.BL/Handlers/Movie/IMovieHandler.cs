using AnticevicApi.Model.Binding.Common;
using System.Collections.Generic;
using View = AnticevicApi.Model.View.Movie;

namespace AnticevicApi.BL.Handlers.Movie
{
    public interface IMovieHandler : IHandler
    {
        IEnumerable<View.Movie> Get(FilteredPagedBinding binding);

        int GetCount(FilteredBinding binding);
    }
}

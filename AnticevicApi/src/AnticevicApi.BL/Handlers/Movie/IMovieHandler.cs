using AnticevicApi.Model.Binding.Movie;
using AnticevicApi.Model.View;
using View = AnticevicApi.Model.View.Movie;
using System;

namespace AnticevicApi.BL.Handlers.Movie
{
    public interface IMovieHandler : IHandler
    {
        PagedView<View.Movie> Get(MovieGetBinding binding);

        int GetCount(MovieGetBinding binding);

        double GetMyRatingAverage(MovieGetBinding binding);

        double GetRatingAverage(MovieGetBinding binding);

        int GetRuntimeAverage(MovieGetBinding binding);

        int GetSum(MovieGetBinding binding, Func<Model.Database.Main.User.Movie, int> selector);
    }
}

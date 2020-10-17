using ProjectIvy.Model.Binding.Movie;
using ProjectIvy.Model.View;
using System;
using System.Collections.Generic;
using View = ProjectIvy.Model.View.Movie;

namespace ProjectIvy.Business.Handlers.Movie
{
    public interface IMovieHandler : IHandler
    {
        PagedView<View.Movie> Get(MovieGetBinding binding);

        View.Movie Get(string imdbId);

        int Count(MovieGetBinding binding);

        IEnumerable<KeyValuePair<int, int>> CountByDayOfWeek(MovieGetBinding binding);

        IEnumerable<KeyValuePair<int, int>> CountByMonth(MovieGetBinding binding);

        IEnumerable<GroupedByMonth<int>> CountByMonthOfYear(MovieGetBinding binding);

        IEnumerable<KeyValuePair<string, int>> CountByMovieDecade(MovieGetBinding binding);

        IEnumerable<KeyValuePair<short, int>> CountByMovieYear(MovieGetBinding binding);

        IEnumerable<KeyValuePair<short, int>> CountByMyRating(MovieGetBinding binding);

        IEnumerable<KeyValuePair<string, int>> CountByRuntime(MovieGetBinding binding);

        IEnumerable<GroupedByYear<int>> CountByYear(MovieGetBinding binding);

        double GetMyRatingAverage(MovieGetBinding binding);

        double GetRatingAverage(MovieGetBinding binding);

        int GetRuntimeAverage(MovieGetBinding binding);

        int GetSum(MovieGetBinding binding, Func<Model.Database.Main.User.Movie, int> selector);
    }
}

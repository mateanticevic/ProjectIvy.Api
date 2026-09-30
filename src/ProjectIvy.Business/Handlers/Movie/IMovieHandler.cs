using System.Threading.Tasks;
using ProjectIvy.Model.Binding.Movie;
using ProjectIvy.Model.View;
using View = ProjectIvy.Model.View.Movie;

namespace ProjectIvy.Business.Handlers.Movie;

public interface IMovieHandler : IHandler
{
    Task<int> Count(MovieGetBinding binding);

    Task<IEnumerable<KeyValuePair<DateTime, int>>> CountByDay(MovieGetBinding binding);

    Task<IEnumerable<KeyValuePair<int, int>>> CountByDayOfWeek(MovieGetBinding binding);

    Task<IEnumerable<KeyValuePair<int, int>>> CountByMonth(MovieGetBinding binding);

    Task<IEnumerable<KeyValuePair<DateTime, int>>> CountByMonthOfYear(MovieGetBinding binding);

    Task<IEnumerable<KeyValuePair<string, int>>> CountByMovieDecade(MovieGetBinding binding);

    Task<IEnumerable<KeyValuePair<short, int>>> CountByMovieYear(MovieGetBinding binding);

    Task<IEnumerable<KeyValuePair<short, int>>> CountByMyRating(MovieGetBinding binding);

    Task<IEnumerable<KeyValuePair<string, int>>> CountByRuntime(MovieGetBinding binding);

    Task<IEnumerable<KeyValuePair<int, int>>> CountByYear(MovieGetBinding binding);

    Task<PagedView<View.Movie>> Get(MovieGetBinding binding);

    Task<View.Movie> Get(string imdbId);

    Task<double> GetMyRatingAverage(MovieGetBinding binding);

    Task<IEnumerable<KeyValuePair<int, decimal>>> GetMyRatingAverageByYear(MovieGetBinding binding);

    Task<double> GetRatingAverage(MovieGetBinding binding);

    Task<IEnumerable<KeyValuePair<int, decimal>>> GetRatingAverageByYear(MovieGetBinding binding);

    Task<int> GetRuntimeAverage(MovieGetBinding binding);

    Task<int> GetSum(MovieGetBinding binding, Func<Model.Database.Main.User.Movie, int> selector);
}

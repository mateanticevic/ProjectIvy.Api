using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using ProjectIvy.Common.Extensions;
using ProjectIvy.Data.Extensions;
using ProjectIvy.Data.Extensions.Entities;
using ProjectIvy.Model.Binding.Movie;
using ProjectIvy.Model.View;
using View = ProjectIvy.Model.View.Movie;
using static System.Math;

namespace ProjectIvy.Business.Handlers.Movie;

public class MovieHandler : Handler<MovieHandler>, IMovieHandler
{
    public static readonly DateTime FirstSunday = new DateTime(2000, 1, 2);

    public MovieHandler(IHandlerContext<MovieHandler> context) : base(context)
    {
    }

    public async Task<int> Count(MovieGetBinding binding)
    {
        using var db = GetMainContext();
        return await db.Movies.WhereUser(UserId)
                        .Where(binding)
                        .CountAsync();
    }

    public async Task<IEnumerable<KeyValuePair<DateTime, int>>> CountByDay(MovieGetBinding binding)
    {
        using var db = GetMainContext();
        return await db.Movies.WhereUser(UserId)
                        .Where(binding)
                        .GroupBy(x => x.Timestamp.Date)
                        .OrderBy(x => x.Key)
                        .Select(x => new KeyValuePair<DateTime, int>(x.Key, x.Count()))
                        .ToListAsync();
    }

    public async Task<IEnumerable<KeyValuePair<int, int>>> CountByDayOfWeek(MovieGetBinding binding)
    {
        using var db = GetMainContext();
        return await db.Movies.WhereUser(UserId)
                        .Where(binding)
                        .GroupBy(x => ((int)EF.Functions.DateDiffDay((DateTime?)FirstSunday, (DateTime?)x.Timestamp) - 1) % 7)
                        .OrderBy(x => x.Key)
                        .Select(x => new KeyValuePair<int, int>(x.Key, x.Count()))
                        .ToListAsync();
    }

    public async Task<IEnumerable<KeyValuePair<int, int>>> CountByMonth(MovieGetBinding binding)
    {
        using var db = GetMainContext();
        return (await db.Movies.WhereUser(UserId)
                        .Where(binding)
                        .GroupBy(x => x.Timestamp.Month)
                        .OrderBy(x => x.Key)
                        .Select(x => new KeyValuePair<int, int>(x.Key, x.Count()))
                        .ToListAsync())
                        .FillMissingMonths();
    }

    public async Task<IEnumerable<KeyValuePair<DateTime, int>>> CountByMonthOfYear(MovieGetBinding binding)
    {
        using var db = GetMainContext();
        return await db.Movies.WhereUser(UserId)
                        .Where(binding)
                        .GroupBy(x => new { x.Timestamp.Year, x.Timestamp.Month })
                        .OrderBy(x => x.Key.Year)
                        .ThenBy(x => x.Key.Month)
                        .Select(x => new KeyValuePair<DateTime, int>(new DateTime(x.Key.Year, x.Key.Month, 1), x.Count()))
                        .ToListAsync();
    }

    public async Task<IEnumerable<KeyValuePair<string, int>>> CountByMovieDecade(MovieGetBinding binding)
    {
        using var db = GetMainContext();
        return await db.Movies.WhereUser(UserId)
                        .Where(binding)
                        .GroupBy(x => x.Year - x.Year % 10)
                        .OrderBy(x => x.Key)
                        .Select(x => new KeyValuePair<string, int>($"{x.Key}'s", x.Count()))
                        .ToListAsync();
    }

    public async Task<IEnumerable<KeyValuePair<short, int>>> CountByMovieYear(MovieGetBinding binding)
    {
        using var db = GetMainContext();
        return await db.Movies.WhereUser(UserId)
                        .Where(binding)
                        .GroupBy(x => x.Year)
                        .OrderBy(x => x.Key)
                        .Select(x => new KeyValuePair<short, int>(x.Key, x.Count()))
                        .ToListAsync();
    }

    public async Task<IEnumerable<KeyValuePair<short, int>>> CountByMyRating(MovieGetBinding binding)
    {
        using var db = GetMainContext();
        return await db.Movies.WhereUser(UserId)
                        .Where(binding)
                        .GroupBy(x => x.MyRating)
                        .OrderBy(x => x.Key)
                        .Select(x => new KeyValuePair<short, int>(x.Key, x.Count()))
                        .ToListAsync();
    }

    public async Task<IEnumerable<KeyValuePair<string, int>>> CountByRuntime(MovieGetBinding binding)
    {
        using var db = GetMainContext();
        return await db.Movies.WhereUser(UserId)
                        .Where(binding)
                        .GroupBy(x => x.Runtime - (x.Runtime % 10) + 10)
                        .OrderBy(x => x.Key)
                        .Select(x => new KeyValuePair<string, int>($"~{x.Key}min", x.Count()))
                        .ToListAsync();
    }

    public async Task<IEnumerable<KeyValuePair<int, int>>> CountByYear(MovieGetBinding binding)
    {
        using var db = GetMainContext();
        return await db.Movies.WhereUser(UserId)
                        .Where(binding)
                        .GroupBy(x => x.Timestamp.Year)
                        .OrderBy(x => x.Key)
                        .Select(x => new KeyValuePair<int, int>(x.Key, x.Count()))
                        .ToListAsync();
    }

    public async Task<PagedView<View.Movie>> Get(MovieGetBinding binding)
    {
        using var db = GetMainContext();
        return await db.Movies.WhereUser(UserId)
                        .Where(binding)
                        .OrderBy(binding)
                        .Select(x => new View.Movie(x))
                        .ToPagedViewAsync(binding);
    }

    public async Task<View.Movie> Get(string imdbId)
    {
        using var context = GetMainContext();
        return (await context.Movies.WhereUser(UserId)
                             .SingleOrDefaultAsync(x => x.ImdbId == imdbId))
                             .ConvertTo(x => new View.Movie(x));
    }

    public async Task<double> GetMyRatingAverage(MovieGetBinding binding)
    {
        using var db = GetMainContext();
        var userMovies = db.Movies.WhereUser(UserId)
                                  .Where(binding);

        double average = (double)await userMovies.SumAsync(x => x.MyRating) / await userMovies.CountAsync();

        return Round(average, 1);
    }

    public async Task<IEnumerable<KeyValuePair<int, decimal>>> GetMyRatingAverageByYear(MovieGetBinding binding)
    {
        using var db = GetMainContext();
        return await db.Movies.WhereUser(UserId)
                              .Where(binding)
                              .GroupBy(x => x.Timestamp.Year)
                              .OrderBy(x => x.Key)
                              .Select(x => new KeyValuePair<int, decimal>(x.Key, (decimal)Round(x.Average(y => y.MyRating), 1)))
                              .ToListAsync();
    }

    public async Task<double> GetRatingAverage(MovieGetBinding binding)
    {
        using var db = GetMainContext();
        var userMovies = db.Movies.WhereUser(UserId)
                                  .Where(binding);

        return Round((double)await userMovies.AverageAsync(x => x.Rating), 1);
    }

    public async Task<IEnumerable<KeyValuePair<int, decimal>>> GetRatingAverageByYear(MovieGetBinding binding)
    {
        using var db = GetMainContext();
        return await db.Movies.WhereUser(UserId)
                              .Where(binding)
                              .GroupBy(x => x.Timestamp.Year)
                              .OrderBy(x => x.Key)
                              .Select(x => new KeyValuePair<int, decimal>(x.Key, Round(x.Average(y => y.Rating), 1)))
                              .ToListAsync();
    }

    public async Task<int> GetRuntimeAverage(MovieGetBinding binding)
    {
        using var db = GetMainContext();
        return (int)await db.Movies.WhereUser(UserId)
                              .Where(binding)
                              .AverageAsync(x => x.Runtime);
    }

    public async Task<int> GetSum(MovieGetBinding binding, Func<Model.Database.Main.User.Movie, int> selector)
    {
        using var db = GetMainContext();
        return (await db.Movies.WhereUser(UserId)
                        .Where(binding)
                        .ToListAsync()).Sum(selector);
    }
}

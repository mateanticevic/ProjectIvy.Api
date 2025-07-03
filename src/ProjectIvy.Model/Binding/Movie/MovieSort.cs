using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

namespace ProjectIvy.Model.Binding.Movie;

[JsonConverter(typeof(StringEnumConverter))]
public enum MovieSort
{
    Watched,
    Rating,
    Runtime,
    MyRating,
    MyRatingDifference,
    Title,
    Year
}

using Newtonsoft.Json.Converters;
using Newtonsoft.Json;

namespace ProjectIvy.Model.Binding.Movie
{
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
}

using System;
using DatabaseModel = ProjectIvy.Model.Database.Main;

namespace ProjectIvy.Model.View.Movie;

public class Movie
{
    public Movie(DatabaseModel.User.Movie x)
    {
        ImdbId = x.ImdbId;
        MyRating = x.MyRating;
        Rating = x.Rating;
        Runtime = x.Runtime;
        Timestamp = x.Timestamp;
        Title = x.Title;
        Year = x.Year;
    }

    public string ImdbId { get; set; }

    public int MyRating { get; set; }

    public decimal Rating { get; set; }

    public int Runtime { get; set; }

    public DateTime Timestamp { get; set; }

    public string Title { get; set; }

    public short Year { get; set; }
}

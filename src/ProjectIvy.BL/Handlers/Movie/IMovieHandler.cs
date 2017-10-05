﻿using ProjectIvy.Model.Binding.Movie;
using ProjectIvy.Model.View;
using View = ProjectIvy.Model.View.Movie;
using System;

namespace ProjectIvy.BL.Handlers.Movie
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
﻿using ProjectIvy.DL.Extensions;
using ProjectIvy.DL.Extensions.Entities;
using ProjectIvy.Model.Binding.Movie;
using ProjectIvy.Model.View;
using System.Collections.Generic;
using System.Linq;
using System;
using View = ProjectIvy.Model.View.Movie;

namespace ProjectIvy.BL.Handlers.Movie
{
    public class MovieHandler : Handler<MovieHandler>, IMovieHandler
    {
        public MovieHandler(IHandlerContext<MovieHandler> context) : base(context)
        {
        }

        public PagedView<View.Movie> Get(MovieGetBinding binding)
        {
            using (var db = GetMainContext())
            {
                var query = db.Movies.WhereUser(User.Id)
                                     .Where(binding);

                var items = query.OrderBy(binding)
                                 .Page(binding.ToPagedBinding())
                                 .ToList()
                                 .Select(x => new View.Movie(x));

                int count = query.Count();

                return new PagedView<View.Movie>(items, count);
            }
        }

        public int GetCount(MovieGetBinding binding)
        {
            using (var db = GetMainContext())
            {
                var userMovies = db.Movies.WhereUser(User.Id)
                                          .Where(binding);

                return userMovies.Count();
            }
        }

        public IEnumerable<GroupedByMonth<int>> GetCountByMonth(MovieGetBinding binding)
        {
            using (var db = GetMainContext())
            {
                return db.Movies.WhereUser(User.Id)
                                .Where(binding)
                                .GroupBy(x => new { x.Timestamp.Year, x.Timestamp.Month })
                                .Select(x => new GroupedByMonth<int>(x.Count(), x.Key.Year, x.Key.Month))
                                .OrderByDescending(x => x.Year)
                                .ThenByDescending(x => x.Month)
                                .ToList();
            }
        }

        public IEnumerable<GroupedByYear<int>> GetCountByYear(MovieGetBinding binding)
        {
            using (var db = GetMainContext())
            {
                return db.Movies.WhereUser(User.Id)
                                .Where(binding)
                                .GroupBy(x => x.Timestamp.Year)
                                .Select(x => new GroupedByYear<int>(x.Count(), x.Key))
                                .OrderByDescending(x => x.Year)
                                .ToList();
            }
        }

        public double GetMyRatingAverage(MovieGetBinding binding)
        {
            using (var db = GetMainContext())
            {
                var userMovies = db.Movies.WhereUser(User.Id)
                                          .Where(binding);

                double average = (double)userMovies.Sum(x => x.MyRating) / userMovies.Count();

                return Math.Round(average, 1);
            }
        }

        public double GetRatingAverage(MovieGetBinding binding)
        {
            using (var db = GetMainContext())
            {
                var userMovies = db.Movies.WhereUser(User.Id)
                                          .Where(binding);

                return Math.Round((double)userMovies.Average(x => x.Rating), 1);
            }
        }

        public int GetRuntimeAverage(MovieGetBinding binding)
        {
            using (var db = GetMainContext())
            {
                var userMovies = db.Movies.WhereUser(User.Id)
                                          .Where(binding);

                return (int)userMovies.Average(x => x.Runtime);
            }
        }

        public int GetSum(MovieGetBinding binding, Func<Model.Database.Main.User.Movie, int> selector)
        {
            using (var db = GetMainContext())
            {
                var userMovies = db.Movies.WhereUser(User.Id)
                                          .Where(binding);

                return userMovies.Sum(selector);
            }
        }
    }
}

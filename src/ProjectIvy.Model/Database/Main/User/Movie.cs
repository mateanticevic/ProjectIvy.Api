﻿using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ProjectIvy.Model.Database.Main.User
{
    [Table(nameof(Movie), Schema = nameof(User))]
    public class Movie : UserEntity, IHasTimestamp
    {
        [Key]
        public int Id { get; set; }

        public DateTime Timestamp { get; set; }

        public short MyRating { get; set; }

        public decimal Rating { get; set; }

        public int Runtime { get; set; }

        public short Year { get; set; }

        public string ImdbId { get; set; }

        public string Title { get; set; }
    }
}

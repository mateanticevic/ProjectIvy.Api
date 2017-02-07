using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System;

namespace AnticevicApi.Model.Database.Main.User
{
    [Table("Movie", Schema = "User")]
    public class Movie : UserEntity, IHasTimestamp
    {
        [Key]
        public int Id { get; set; }

        public DateTime Timestamp { get; set; }

        public int MyRating { get; set; }

        public decimal Rating { get; set; }

        public int Runtime { get; set; }

        public short Year { get; set; }

        public string ImdbId { get; set; }

        public string Title { get; set; }
    }
}

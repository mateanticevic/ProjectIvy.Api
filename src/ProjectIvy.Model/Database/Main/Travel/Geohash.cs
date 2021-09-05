using System.ComponentModel.DataAnnotations.Schema;

namespace ProjectIvy.Model.Database.Main.Travel
{
    [Table(nameof(Geohash), Schema = nameof(Travel))]
    public class Geohash : UserEntity
    {
        public long Id { get; set; }

        public string Hash { get; set; }
    }
}

using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ProjectIvy.Model.Database.Main.User
{
    [Table(nameof(Weight), Schema = nameof(User))]
    public class Weight : UserEntity
    {
        [Key]
        public int Id { get; set; }

        public DateTime Date { get; set; }

        public decimal Value { get; set; }
    }
}

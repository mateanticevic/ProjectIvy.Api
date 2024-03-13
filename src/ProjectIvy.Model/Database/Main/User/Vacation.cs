using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ProjectIvy.Model.Database.Main.User
{
    [Table(nameof(Vacation), Schema = nameof(User))]
    public class Vacation : UserEntity
    {
        [Key]
        public int Id { get; set; }

        public DateTime Date { get; set; }
    }
}

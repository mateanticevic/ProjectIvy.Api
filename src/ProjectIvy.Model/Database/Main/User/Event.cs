using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ProjectIvy.Model.Database.Main.User
{
    [Table(nameof(Event), Schema = nameof(User))]
    public class Event : UserEntity, IHasValueId
    {
        [Key]
        public int Id { get; set; }

        public DateTime Date { get; set; }

        public string ValueId { get; set; }

        public string Name { get; set; }
    }
}

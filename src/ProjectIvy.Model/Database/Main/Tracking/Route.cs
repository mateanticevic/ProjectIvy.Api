using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ProjectIvy.Model.Database.Main.Tracking
{
    [Table(nameof(Route), Schema = nameof(Tracking))]
    public class Route : UserEntity
    {
        [Key]
        public int Id { get; set; }

        public string Name { get; set; }
        
        public string ValueId { get; set; }
    }
}

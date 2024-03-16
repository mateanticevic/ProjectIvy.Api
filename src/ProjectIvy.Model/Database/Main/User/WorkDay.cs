using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ProjectIvy.Model.Database.Main.User
{
    [Table(nameof(WorkDay), Schema = nameof(User))]
    public class WorkDay : UserEntity
    {
        [Key]
        public int Id { get; set; }

        public DateTime Date { get; set; }

        public int WorkDayTypeId { get; set; }

        public WorkDayType WorkDayType { get; set; }
    }
}

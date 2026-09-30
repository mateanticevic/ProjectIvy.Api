using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ProjectIvy.Model.Database.Main.User;

[Table(nameof(WorkDay), Schema = nameof(User))]
public class WorkDay : UserEntity
{
    public DateTime Date { get; set; }

    [Key]
    public int Id { get; set; }

    public WorkDayType WorkDayType { get; set; }

    public int WorkDayTypeId { get; set; }
}

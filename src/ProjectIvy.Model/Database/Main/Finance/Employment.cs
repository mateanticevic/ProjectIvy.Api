using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ProjectIvy.Model.Database.Main.Finance;

[Table(nameof(Employment), Schema = nameof(Finance))]
public class Employment : UserEntity
{
    [Key]
    public int Id { get; set; }

    public DateTime From { get; set; }

    public DateTime? To { get; set; }

    public int CompanyId { get; set; }

    public int DefaultWorkDayTypeId { get; set; }

    public Common.Company Company { get; set; }

    public User.WorkDayType DefaultWorkDayType { get; set; }
}

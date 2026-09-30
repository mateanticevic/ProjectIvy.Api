using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ProjectIvy.Model.Database.Main.Common;

[Table(nameof(Currency), Schema = nameof(Common))]
public class Currency : IHasValueId
{
    public string Code { get; set; }

    [Key]
    public int Id { get; set; }

    public string Name { get; set; }

    public string Symbol { get; set; }

    public ICollection<User.User> Users { get; set; }

    public string ValueId { get; set; }
}

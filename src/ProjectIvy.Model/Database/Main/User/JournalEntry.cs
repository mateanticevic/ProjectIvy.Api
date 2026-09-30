using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ProjectIvy.Model.Database.Main.User;

[Table(nameof(JournalEntry), Schema = nameof(User))]
public class JournalEntry : UserEntity, IHasCreatedModified
{
    public DateTime Created { get; set; }

    public DateOnly Date { get; set; }

    public string Entry { get; set; }

    [Key]
    public long Id { get; set; }

    public DateTime Modified { get; set; }
}

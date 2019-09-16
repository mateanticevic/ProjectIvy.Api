using System.ComponentModel.DataAnnotations.Schema;

namespace ProjectIvy.Model.Database.Main.Contacts
{
    [Table(nameof(Contact), Schema = nameof(Contacts))]
    public class CallBlacklist : UserEntity
    {
        public string Number { get; set; }
    }
}

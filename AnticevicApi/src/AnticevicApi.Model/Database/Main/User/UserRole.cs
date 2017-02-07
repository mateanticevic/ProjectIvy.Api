using System.ComponentModel.DataAnnotations.Schema;

namespace AnticevicApi.Model.Database.Main.User
{
    [Table("UserRole", Schema = "User")]
    public class UserRole
    {
        public int UserId { get; set; }

        public User User { get; set; }

        public int RoleId { get; set; }

        public Role Role { get; set; }
    }
}

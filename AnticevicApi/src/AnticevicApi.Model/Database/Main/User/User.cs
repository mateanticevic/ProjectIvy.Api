using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace AnticevicApi.Model.Database.Main.User
{
    [Table("User", Schema = "User")]
    public class User
    {
        [Key]
        public int Id { get; set; }

        public string FirstName { get; set; }

        public string LastName { get; set; }

        public string Email { get; set; }

        public string PasswordHash { get; set; }

        public string Username { get; set; }

        public string LastFmUsername { get; set; }

        public ICollection<UserRole> UserRoles { get; set; }

        public ICollection<Tracking.Tracking> Trackings { get; set; }
    }
}

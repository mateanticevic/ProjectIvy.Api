using System;
using System.ComponentModel.DataAnnotations.Schema;

namespace ProjectIvy.Model.Database.Main.User
{
    [Table(nameof(UserModule), Schema = nameof(User))]
    public class UserModule
    {
        public int UserId { get; set; }

        public int ModuleId { get; set; }

        public bool IsActive { get; set; }

        public DateTime AvailableFrom { get; set; }

        public User User { get; set; }

        public App.Module Module { get; set; }
    }
}

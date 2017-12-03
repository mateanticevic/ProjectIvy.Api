using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System;

namespace ProjectIvy.Model.Database.Main.User
{
    [Table(nameof(User), Schema = nameof(User))]
    public class User : IHasCreatedModified
    {
        [Key]
        public int Id { get; set; }

        public int DefaultCurrencyId { get; set; }

        public int DefaultLanguageId { get; set; }

        public string FirstName { get; set; }

        public string LastName { get; set; }

        public string Email { get; set; }

        public string PasswordHash { get; set; }

        public DateTime PasswordModified { get; set; }

        public string Username { get; set; }

        public string LastFmUsername { get; set; }

        public DateTime Created { get; set; }

        public DateTime Modified { get; set; }

        public Common.Currency DefaultCurrency { get; set; }

        public ICollection<UserRole> UserRoles { get; set; }

        public ICollection<Tracking.Tracking> Trackings { get; set; }
    }
}

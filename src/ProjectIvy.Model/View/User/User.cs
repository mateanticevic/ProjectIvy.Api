using System.Collections.Generic;
using System.Linq;
using DatabaseModel = ProjectIvy.Model.Database.Main;

namespace ProjectIvy.Model.View.User
{
    public class User
    {
        public User(DatabaseModel.User.User x)
        {
            DefaultCurrency = new Currency.Currency(x.DefaultCurrency);
            FirstName = x.FirstName;
            LastName = x.LastName;
            Email = x.Email;
            Username = x.Username;
            Roles = x.UserRoles.Select(y => new Role.Role(y.Role));
        }

        public Currency.Currency DefaultCurrency { get; set; }

        public string FirstName { get; set; }

        public string LastName { get; set; }

        public string Email { get; set; }

        public string Username { get; set; }

        public IEnumerable<Role.Role> Roles { get; set; }
    }
}

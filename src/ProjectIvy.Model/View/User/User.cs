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
            Modules = x.Modules.Where(y => y.IsActive).Select(y => y.Module.ValueId);
        }

        public Currency.Currency DefaultCurrency { get; set; }

        public string FirstName { get; set; }

        public string LastName { get; set; }

        public string Email { get; set; }

        public string Username { get; set; }

        public IEnumerable<string> Modules { get; set; }
    }
}

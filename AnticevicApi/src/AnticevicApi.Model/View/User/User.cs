using DatabaseModel = AnticevicApi.Model.Database.Main;
using System.Collections.Generic;
using System.Linq;

namespace AnticevicApi.Model.View.User
{
    public class User
    {
        public User(DatabaseModel.User.User x)
        {
            FirstName = x.FirstName;
            LastName = x.LastName;
            Roles = x.UserRoles.Select(y => new Role.Role(y.Role));
        }

        public string FirstName { get; set; }

        public string LastName { get; set; }

        public IEnumerable<Role.Role> Roles { get; set; }
    }
}

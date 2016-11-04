using AnticevicApi.DL.DbContexts;
using AnticevicApi.Model.Database.Main.User;
using Microsoft.EntityFrameworkCore;
using System.Linq;

namespace AnticevicApi.DL.Helpers
{
    public class UserHelper
    {
        public static User Get(string username)
        {
            using (var db = new MainContext())
            {
                return db.Users.Include(x => x.UserRoles)
                               .ThenInclude(x => x.Role)
                               .SingleOrDefault(x => x.Username == username);
            }
        }
    }
}

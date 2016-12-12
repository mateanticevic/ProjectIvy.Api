using AnticevicApi.DL.DbContexts;
using Microsoft.EntityFrameworkCore;
using System.Linq;
using View = AnticevicApi.Model.View.User;

namespace AnticevicApi.BL.Handlers.User
{
    public class UserHandler : Handler, IUserHandler
    {
        public View.User Get(string username)
        {
            using (var db = new MainContext(ConnectionString))
            {
                var userEntity = db.Users.Include(x => x.UserRoles)
                                         .ThenInclude(x => x.Role)
                                         .SingleOrDefault(x => x.Username == username);

                return new View.User(userEntity);
            }
        }

        public View.User Get(int? id = null)
        {
            id = id.HasValue ? id : UserId;

            using (var db = new MainContext(ConnectionString))
            {
                var userEntity = db.Users.Include(x => x.UserRoles)
                                         .ThenInclude(x => x.Role)
                                         .SingleOrDefault(x => x.Id == id);

                return new View.User(userEntity);
            }
        }
    }
}

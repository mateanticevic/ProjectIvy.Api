using AnticevicApi.DL.DbContexts;
using AnticevicApi.Model.View.User;
using Microsoft.EntityFrameworkCore;
using System.Linq;

namespace AnticevicApi.BL.Handlers
{
    public class UserHandler : Handler
    {
        public UserHandler(string connectionString, int userId) : base(connectionString, userId)
        {
            UserId = userId;
        }

        public User Get(string username)
        {
            using (var db = new MainContext(ConnectionString))
            {
                var userEntity = db.Users.Include(x => x.UserRoles)
                                    .ThenInclude(x => x.Role)
                                    .SingleOrDefault(x => x.Username == username);

                return new User(userEntity);
            }
        }
    }
}

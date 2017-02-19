using AnticevicApi.Common.Helpers;
using AnticevicApi.DL.Extensions;
using AnticevicApi.Model.Binding.User;
using Microsoft.EntityFrameworkCore;
using System.Linq;
using System;
using View = AnticevicApi.Model.View.User;

namespace AnticevicApi.BL.Handlers.User
{
    public class UserHandler : Handler<UserHandler>, IUserHandler
    {
        public UserHandler(IHandlerContext<UserHandler> context) : base(context)
        {
        }

        public View.User Get(string username)
        {
            using (var db = GetMainContext())
            {
                var userEntity = db.Users.Include(x => x.UserRoles)
                                         .ThenInclude(x => x.Role)
                                         .SingleOrDefault(x => x.Username == username);

                return new View.User(userEntity);
            }
        }

        public View.User Get(int? id = null)
        {
            id = id.HasValue ? id : User.Id;

            using (var db = GetMainContext())
            {
                var userEntity = db.Users.Include(x => x.UserRoles)
                                         .ThenInclude(x => x.Role)
                                         .SingleOrDefault(x => x.Id == id);

                return new View.User(userEntity);
            }
        }

        public void SetPassword(PasswordSetBinding binding)
        {
            using (var db = GetMainContext())
            {
                var userEntity = db.Users.GetById(User.Id);
                userEntity.PasswordHash = PasswordHelper.GetHash(binding.Password);
                userEntity.PasswordModified = DateTime.Now;

                db.SaveChanges();
            }
        }
    }
}

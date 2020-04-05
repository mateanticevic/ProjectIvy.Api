﻿using Microsoft.EntityFrameworkCore;
using ProjectIvy.Common.Helpers;
using ProjectIvy.Data.Extensions;
using ProjectIvy.Model.Binding.User;
using ProjectIvy.Model.Database.Main.User;
using System;
using System.Linq;
using System.Threading.Tasks;
using View = ProjectIvy.Model.View.User;

namespace ProjectIvy.Business.Handlers.User
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
                                         .Include(x => x.DefaultCurrency)
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

        public async Task SetWeight(decimal weight)
        {
            using (var context = GetMainContext())
            {
                var entity = new Weight()
                {
                    UserId = User.Id,
                    Date = DateTime.Now,
                    Value = weight
                };

                await context.Weights.AddAsync(entity);
            }
        }
    }
}

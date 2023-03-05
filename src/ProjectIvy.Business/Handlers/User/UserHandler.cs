using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using ProjectIvy.Business.MapExtensions;
using ProjectIvy.Data.Extensions;
using ProjectIvy.Model.Binding.User;
using ProjectIvy.Model.Database.Main.User;
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
                var userEntity = db.Users.SingleOrDefault(x => x.Username == username);

                return new View.User(userEntity);
            }
        }

        public View.User Get(int? id = null)
        {
            id = id.HasValue ? id : UserId;

            using (var db = GetMainContext())
            {
                var userEntity = db.Users.Include(x => x.DefaultCar)
                                         .Include(x => x.DefaultCurrency)
                                         .Include(x => x.DefaultCar.CarModel)
                                         .Include("Modules.Module")
                                         .SingleOrDefault(x => x.Id == id);

                return new View.User(userEntity);
            }
        }

        public async Task Update(UserUpdateBinding binding)
        {
            using (var context = GetMainContext())
            {
                var user = await context.Users.SingleOrDefaultAsync(x => x.Id == UserId);
                context.Update(binding.ToEntity(context, user));

                await context.SaveChangesAsync();
            }
        }

        public async Task SetWeight(decimal weight)
        {
            using (var context = GetMainContext())
            {
                var entity = new Weight()
                {
                    UserId = UserId,
                    Date = DateTime.Now,
                    Value = weight
                };

                await context.Weights.AddAsync(entity);
                await context.SaveChangesAsync();
            }
        }
    }
}

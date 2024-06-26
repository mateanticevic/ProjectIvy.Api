﻿using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Memory;
using ProjectIvy.Business.Caching;
using ProjectIvy.Business.MapExtensions;
using ProjectIvy.Data.Extensions;
using ProjectIvy.Model.Binding;
using ProjectIvy.Model.Binding.User;
using ProjectIvy.Model.Database.Main.User;
using View = ProjectIvy.Model.View.User;

namespace ProjectIvy.Business.Handlers.User
{
    public class UserHandler : Handler<UserHandler>, IUserHandler
    {


        public UserHandler(IHandlerContext<UserHandler> context,
                           IMemoryCache memoryCache) : base(context, memoryCache, nameof(UserHandler))
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
            => MemoryCache.GetOrCreate(BuildUserCacheKey(CacheKeyGenerator.UserGet()),
                cacheEntry =>
                {
                    cacheEntry.AbsoluteExpirationRelativeToNow = TimeSpan.FromMinutes(10);
                    return GetNonCached(id);
                });

        public async Task<IEnumerable<KeyValuePair<DateTime, decimal>>> GetWeight(FilteredBinding b)
        {
            using var context = GetMainContext();

            return await context.Weights.WhereUser(UserId)
                                        .WhereIf(b.From.HasValue, x => x.Date >= b.From)
                                        .WhereIf(b.To.HasValue, x => x.Date <= b.To)
                                        .OrderByDescending(x => x.Date)
                                        .Select(x => new KeyValuePair<DateTime, decimal>(x.Date, x.Value))
                                        .ToListAsync();
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

        private View.User GetNonCached(int? id = null)
        {
            id = id.HasValue ? id : UserId;

            using (var db = GetMainContext())
            {
                var userEntity = db.Users.Include(x => x.DefaultCar)
                                         .Include(x => x.DefaultCurrency)
                                         .Include(x => x.DefaultCar.CarModel)
                                         .SingleOrDefault(x => x.Id == id);

                return new View.User(userEntity);
            }
        }
    }
}

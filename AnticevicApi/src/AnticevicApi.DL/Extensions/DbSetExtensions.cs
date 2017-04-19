﻿using AnticevicApi.Model.Database.Main.Finance;
using AnticevicApi.Model.Database.Main.Inv;
using AnticevicApi.Model.Database.Main.Net;
using AnticevicApi.Model.Database.Main.Org;
using AnticevicApi.Model.Database.Main.Transport;
using AnticevicApi.Model.Database.Main.User;
using Microsoft.EntityFrameworkCore;
using System.Linq;

namespace AnticevicApi.DL.Extensions
{
    public static class DbSetExtensions
    {
        public static int? GetId(this DbSet<Car> set, string valueId)
        {
            return set.SingleOrDefault(x => x.ValueId == valueId)?.Id;
        }

        public static int? GetId(this DbSet<Device> set, string valueId)
        {
            return set.SingleOrDefault(x => x.ValueId == valueId)?.Id;
        }

        public static int? GetId(this DbSet<Domain> set, string valueId)
        {
            return set.SingleOrDefault(x => x.ValueId == valueId)?.Id;
        }

        public static int? GetId(this DbSet<ExpenseType> set, string valueId)
        {
            return set.SingleOrDefault(x => x.ValueId == valueId)?.Id;
        }

        public static int? GetId(this DbSet<Vendor> set, string valueId)
        {
            return set.SingleOrDefault(x => x.ValueId == valueId)?.Id;
        }

        public static int? GetId(this DbSet<Project> set, string valueId)
        {
            return set.SingleOrDefault(x => x.ValueId == valueId)?.Id;
        }

        public static User GetById(this DbSet<User> set, int id)
        {
            return set.SingleOrDefault(x => x.Id == id);
        }

        public static int? GetId(this DbSet<Web> set, string valueId)
        {
            return set.SingleOrDefault(x => x.ValueId == valueId)?.Id;
        }
    }
}

﻿using DatabaseModel = AnticevicApi.Model.Database.Main;

namespace AnticevicApi.Model.View.Role
{
    public class Role
    {
        public Role(DatabaseModel.User.Role x)
        {
            Name = x.Name;
            Id = x.ValueId;
        }

        public string Id { get; set; }

        public string Name { get; set; }
    }
}

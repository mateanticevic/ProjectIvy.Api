﻿namespace AnticevicApi.BL.Handlers.Security
{
    public interface ISecurityHandler
    {
        Model.Database.Main.User.User GetUser(string token);
    }
}

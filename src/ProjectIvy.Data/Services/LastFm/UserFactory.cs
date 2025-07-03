using System;
using ProjectIvy.Common.Interfaces;

namespace ProjectIvy.Data.Services.LastFm;

public class UserFactory : IServiceFactory<IUserHelper>
{
    public IUserHelper Build()
    {
        return new UserHelper(Environment.GetEnvironmentVariable("LAST_FM_KEY"));
    }
}

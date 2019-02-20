using System.Collections.Generic;

namespace ProjectIvy.Business.Handlers.Application
{
    public interface IApplicationHandler : IHandler
    {
        Dictionary<string, object> GetSettings(string applicationValueId);
    }
}

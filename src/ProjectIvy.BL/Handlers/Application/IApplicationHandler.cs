using System.Collections.Generic;

namespace ProjectIvy.BL.Handlers.Application
{
    public interface IApplicationHandler : IHandler
    {
        Dictionary<string, object> GetSettings(string applicationValueId);
    }
}

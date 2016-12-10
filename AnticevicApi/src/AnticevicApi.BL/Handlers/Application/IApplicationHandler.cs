using System.Collections.Generic;

namespace AnticevicApi.BL.Handlers.Application
{
    public interface IApplicationHandler : IHandler
    {
        Dictionary<string, object> GetSettings(string applicationValueId);
    }
}

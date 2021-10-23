using System.Collections.Generic;
using System.Threading.Tasks;

namespace ProjectIvy.Business.Handlers.Security
{
    public interface IAccessTokenHandler
    {
        Task<IDictionary<string, string>> GetBearers();
    }
}
using AnticevicApi.Model.Services.LastFm;
using System.Threading.Tasks;

namespace AnticevicApi.DL.Services.LastFm
{
    public interface IUserHelper
    {
        Task<Info> GetTotalCount(string username);
    }
}

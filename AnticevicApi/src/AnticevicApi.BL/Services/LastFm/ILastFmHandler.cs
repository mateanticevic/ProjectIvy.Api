using AnticevicApi.BL.Handlers;
using System.Threading.Tasks;

namespace AnticevicApi.BL.Services.LastFm
{
    public interface ILastFmHandler : IHandler
    {
        Task<int> GetTotalCount();
    }
}

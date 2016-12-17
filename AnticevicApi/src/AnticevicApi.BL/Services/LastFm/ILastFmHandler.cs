using AnticevicApi.BL.Handlers;
using AnticevicApi.Model.Binding.Common;
using AnticevicApi.Model.View.Services.LastFm;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace AnticevicApi.BL.Services.LastFm
{
    public interface ILastFmHandler : IHandler
    {
        Task<int> GetTotalCount();

        Task<IEnumerable<Track>> GetTracks(FilteredPagedBinding binding);
    }
}

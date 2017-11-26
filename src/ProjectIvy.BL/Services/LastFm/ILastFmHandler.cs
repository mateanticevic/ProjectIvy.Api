using ProjectIvy.BL.Handlers;
using ProjectIvy.Model.Binding.Common;
using ProjectIvy.Model.View.Services.LastFm;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace ProjectIvy.BL.Services.LastFm
{
    public interface ILastFmHandler : IHandler
    {
        Task<int> GetTotalCount();

        Task<IEnumerable<Track>> GetTopTracks();

        Task<IEnumerable<Track>> GetTracks(FilteredPagedBinding binding);
    }
}

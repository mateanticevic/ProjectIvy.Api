using AnticevicApi.Model.Binding.Common;
using AnticevicApi.Model.Services.LastFm;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace AnticevicApi.DL.Services.LastFm
{
    public interface IUserHelper
    {
        Task<Info> GetTotalCount(string username);

        Task<IEnumerable<Track>> GetTracks(string username, FilteredPagedBinding filter);
    }
}

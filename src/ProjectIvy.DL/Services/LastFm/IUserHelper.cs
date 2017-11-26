using ProjectIvy.Model.Binding.Common;
using ProjectIvy.Model.Services.LastFm;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace ProjectIvy.DL.Services.LastFm
{
    public interface IUserHelper
    {
        Task<Info> GetTotalCount(string username);

        Task<IEnumerable<Track>> GetLovedTracks(string username);

        Task<IEnumerable<Track>> GetTopTracks(string username);

        Task<IEnumerable<Track>> GetTracks(string username, FilteredPagedBinding filter);
    }
}

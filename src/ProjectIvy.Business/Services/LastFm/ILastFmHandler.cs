using System.Collections.Generic;
using System.Threading.Tasks;
using ProjectIvy.Business.Handlers;
using ProjectIvy.Model.Binding;
using ProjectIvy.Model.View.Services.LastFm;

namespace ProjectIvy.Business.Services.LastFm;

public interface ILastFmHandler : IHandler
{
    Task<int> GetTotalCount();

    Task<IEnumerable<Track>> GetLovedTracks();

    Task<IEnumerable<Artist>> GetTopArtists();

    Task<IEnumerable<Track>> GetTopTracks();

    Task<IEnumerable<Track>> GetTracks(FilteredPagedBinding binding);
}

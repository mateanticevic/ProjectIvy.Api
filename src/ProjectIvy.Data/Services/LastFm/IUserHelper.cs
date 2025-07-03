﻿using System.Collections.Generic;
using System.Threading.Tasks;
using ProjectIvy.Model.Binding;
using ProjectIvy.Model.Services.LastFm;

namespace ProjectIvy.Data.Services.LastFm;

public interface IUserHelper
{
    Task<Info> GetTotalCount(string username);

    Task<IEnumerable<Track>> GetLovedTracks(string username);

    Task<IEnumerable<Artist>> GetTopArtists(string username);

    Task<IEnumerable<Track>> GetTopTracks(string username);

    Task<IEnumerable<Track>> GetTracks(string username, FilteredPagedBinding filter);
}

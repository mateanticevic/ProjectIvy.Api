using ServiceModel = ProjectIvy.Model.Services.LastFm;
using System;

namespace ProjectIvy.Model.View.Services.LastFm
{
    public class Artist
    {
        public Artist(ServiceModel.Artist artist)
        {
            Name = artist.ArtistName;
            PlayCount = Int32.Parse(artist.Playcount);
            Rank = Int32.Parse(artist.Attributes.Rank);
            Url = artist.Url;
        }

        public string Name { get; set; }

        public int PlayCount { get; set; }

        public int Rank { get; set; }

        public string Url { get; set; }
    }
}

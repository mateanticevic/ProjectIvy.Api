using ServiceModel = ProjectIvy.Model.Services.LastFm;

namespace ProjectIvy.Model.View.Services.LastFm
{
    public class Artist
    {
        public Artist(ServiceModel.Artist artist)
        {
            Name = artist.ArtistName;
            PlayCount = int.Parse(artist.Playcount);
            Rank = int.Parse(artist.Attributes.Rank);
            Url = artist.Url;
        }

        public string Name { get; set; }

        public int PlayCount { get; set; }

        public int Rank { get; set; }

        public string Url { get; set; }
    }
}

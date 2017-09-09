using ServiceModel = ProjectIvy.Model.Services.LastFm;

namespace ProjectIvy.Model.View.Services.LastFm
{
    public class Artist
    {
        public Artist(ServiceModel.Artist artist)
        {
            Name = artist.ArtistName;
        }

        public string Name { get; set; }
    }
}

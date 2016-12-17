using ServiceModel = AnticevicApi.Model.Services.LastFm;

namespace AnticevicApi.Model.View.Services.LastFm
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

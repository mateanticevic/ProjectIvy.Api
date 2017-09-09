using System.Linq;
using ServiceModel = ProjectIvy.Model.Services.LastFm;

namespace ProjectIvy.Model.View.Services.LastFm
{
    public class Images
    {
        public Images(ServiceModel.Track track)
        {
            Small = track.Images.SingleOrDefault(x => x.Size == "small")?.Url;
            Medium = track.Images.SingleOrDefault(x => x.Size == "medium")?.Url;
            Large = track.Images.SingleOrDefault(x => x.Size == "large")?.Url;
            ExtraLarge = track.Images.SingleOrDefault(x => x.Size == "extralarge")?.Url;
        }

        public string Small { get; set; }

        public string Medium { get; set; }

        public string Large { get; set; }

        public string ExtraLarge { get; set; }
    }
}

using ProjectIvy.Common.Helpers;
using ServiceModel = ProjectIvy.Model.Services.LastFm;
using System;

namespace ProjectIvy.Model.View.Services.LastFm
{
    public class Track
    {
        public Track(ServiceModel.Track track)
        {
            Artist = new Artist(track.Artist);
            Images = new Images(track);
            Title = track.Name;
            Timestamp = track.Date == null ? (DateTime?)null : DateTimeHelper.FromUnix(track.Date.Unix);
        }

        public Artist Artist { get; set; }

        public Images Images { get; set; }

        public string Title { get; set; }

        public DateTime? Timestamp { get; set; }
    }
}

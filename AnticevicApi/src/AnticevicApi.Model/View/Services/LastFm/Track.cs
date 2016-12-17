using AnticevicApi.Common.Helpers;
using ServiceModel = AnticevicApi.Model.Services.LastFm;
using System;

namespace AnticevicApi.Model.View.Services.LastFm
{
    public class Track
    {
        public Track(ServiceModel.Track track)
        {
            Artist = new Artist(track.Artist);
            Images = new Images(track);
            Title = track.Name;
            Timestamp = DateTimeHelper.FromUnix(track.Date.Unix);
        }
        public Artist Artist { get; set; }

        public Images Images { get; set; }

        public string Title { get; set; }

        public DateTime Timestamp { get; set; }
    }
}

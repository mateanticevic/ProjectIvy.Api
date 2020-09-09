using ProjectIvy.Model.Binding.Common;

namespace ProjectIvy.Model.Binding.Tracking
{
    public class TrackingGetBinding : FilteredBinding
    {
        public LocationBinding BottomRight { get; set; }

        public LocationBinding TopLeft { get; set; }
    }
}

namespace ProjectIvy.Model.Binding.Tracking;

public class TrackingBinding
{
    public decimal? Accuracy { get; set; }

    public decimal? Altitude { get; set; }

    public decimal Latitude { get; set; }

    public decimal Longitude { get; set; }

    public DateTime Timestamp { get; set; }

    public decimal? Speed { get; set; }
}

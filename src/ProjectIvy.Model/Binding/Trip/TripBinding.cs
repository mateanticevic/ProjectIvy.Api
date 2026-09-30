namespace ProjectIvy.Model.Binding.Trip;

public class TripBinding
{
    public IEnumerable<string> CityIds { get; set; }

    public string Name { get; set; }

    public DateTime TimestampEnd { get; set; }

    public DateTime TimestampStart { get; set; }
}

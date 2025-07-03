namespace ProjectIvy.Model.Binding.Flight;

public class FlightGetBinding : FilteredPagedBinding
{
    public string DestinationId { get; set; }

    public string OriginId { get; set; }
}

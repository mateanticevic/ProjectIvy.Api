namespace ProjectIvy.Model.Binding.Stay;

public class StayGetBinding : FilteredPagedBinding
{
    public IEnumerable<string> CityId { get; set; }

    public IEnumerable<string> CountryId { get; set; }

    public StaySort OrderBy { get; set; }
}
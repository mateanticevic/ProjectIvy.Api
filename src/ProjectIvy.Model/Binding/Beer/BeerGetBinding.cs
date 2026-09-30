namespace ProjectIvy.Model.Binding.Beer;

public class BeerGetBinding : PagedBinding, IOrderable<BeerSort>
{
    public string BrandId { get; set; }

    public bool OrderAscending { get; set; } = true;

    public BeerSort OrderBy { get; set; } = BeerSort.Name;

    public string Search { get; set; }
}

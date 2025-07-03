namespace ProjectIvy.Model.Binding.Beer;

public class BeerGetBinding : PagedBinding, IOrderable<BeerSort>
{
    public bool OrderAscending { get; set; } = true;

    public BeerSort OrderBy { get; set; } = BeerSort.Name;

    public string BrandId { get; set; }

    public string Search { get; set; }
}

namespace ProjectIvy.Model.Binding.Beer
{
    public class BeerGetBinding : PagedBinding, IOrderable<BeerSort>
    {
        public bool OrderAscending { get; set; }

        public BeerSort OrderBy { get; set; } = BeerSort.Name;
    }
}

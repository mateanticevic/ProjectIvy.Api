namespace ProjectIvy.Model.View;

public class PagedView<T>
{
    public PagedView()
    {
    }

    public PagedView(IEnumerable<T> items, long count)
    {
        Count = count;
        Items = items;
    }

    public long Count { get; set; }

    public IEnumerable<T> Items { get; set; }
}

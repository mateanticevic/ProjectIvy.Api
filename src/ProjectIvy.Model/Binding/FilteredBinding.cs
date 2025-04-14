namespace ProjectIvy.Model.Binding;

public class FilteredBinding : IFilteredBinding
{
    public FilteredBinding()
    {
    }

    public FilteredBinding(DateTime? from, DateTime? to)
    {
        From = from;
        To = to;
    }

    public DateTime? From { get; set; }

    public DateTime? To { get; set; }

    public int? Last
    {
        set => From = DateTime.Now.AddDays(-1 * value.Value);
    }

    public bool OrderAscending { get; set; }

    public T OverrideFromTo<T>(DateTime? from, DateTime? to) where T : FilteredBinding
    {
        From = from;
        To = to;

        return (T)this;
    }
}

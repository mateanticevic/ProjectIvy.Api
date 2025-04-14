namespace ProjectIvy.Model.Binding;

public class PagedBinding : IPagedBinding
{
    public bool PageAll { get; set; }

    public int Page { get; set; } = 0;

    public int PageSize { get; set; } = 10;
}

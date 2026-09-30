namespace ProjectIvy.Model.Binding;

public class PagedBinding : IPagedBinding
{
    public int Page { get; set; } = 0;

    public bool PageAll { get; set; }

    public int PageSize { get; set; } = 10;
}

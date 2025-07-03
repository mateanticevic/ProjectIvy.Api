namespace ProjectIvy.Model.Binding;

public interface IPagedBinding
{
    bool PageAll { get; set; }

    int Page { get; set; }

    int PageSize { get; set; }
}

namespace ProjectIvy.Model.Binding;

public interface IPagedBinding
{
    int Page { get; set; }

    bool PageAll { get; set; }

    int PageSize { get; set; }
}

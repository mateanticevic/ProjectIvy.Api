using System;

namespace ProjectIvy.Model.Binding;

public class FilteredPagedBinding : FilteredBinding, IPagedBinding, IFilteredBinding
{
    public int Page { get; set; } = 0;

    public bool PageAll { get; set; }

    public int PageSize { get; set; } = 10;
}

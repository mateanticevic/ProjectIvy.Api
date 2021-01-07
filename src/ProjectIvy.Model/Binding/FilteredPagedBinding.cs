using System;

namespace ProjectIvy.Model.Binding
{
    public class FilteredPagedBinding : FilteredBinding, IPagedBinding, IFilteredBinding
    {
        public bool PageAll { get; set; }

        public int Page { get; set; } = 0;

        public int PageSize { get; set; } = 10;
    }
}

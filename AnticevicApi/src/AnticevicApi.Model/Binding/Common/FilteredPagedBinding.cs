using System;

namespace AnticevicApi.Model.Binding.Common
{
    public class FilteredPagedBinding : FilteredBinding
    {
        public FilteredPagedBinding()
        {
        }

        public FilteredPagedBinding(DateTime? from, DateTime? to, int? page, int? pageSize) : base(from, to)
        {
        }

        public int Page { get; set; } = 0;

        public int PageSize { get; set; } = 10;

        public PagedBinding ToPagedBinding()
        {
            return new PagedBinding(Page, PageSize);
        }
    }
}

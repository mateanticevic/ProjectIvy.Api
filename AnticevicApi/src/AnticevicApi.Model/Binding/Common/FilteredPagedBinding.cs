using System;

namespace AnticevicApi.Model.Binding.Common
{
    public class FilteredPagedBinding : FilteredBinding
    {
        public FilteredPagedBinding(DateTime? from, DateTime? to, int? page, int? pageSize) : base(from, to)
        {
            Page = page.HasValue ? page : 0;
            PageSize = pageSize.HasValue ? pageSize : 10;
        }

        public PagedBinding ToPagedBinding()
        {
            return new PagedBinding(Page, PageSize);
        }

        public int? Page { get; set; }

        public int? PageSize { get; set; }
    }
}

using System;

namespace ProjectIvy.Model.Binding
{
    public class FilteredPagedBinding : IPagedBinding, IFilteredBinding
    {
        public DateTime? From { get; set; }

        public DateTime? To { get; set; }

        public bool OrderAscending { get; set; }

        public int Page { get; set; } = 0;

        public int PageSize { get; set; } = 10;

        public T OverrideFromTo<T>(DateTime? from, DateTime? to) where T : FilteredPagedBinding
        {
            From = from;
            To = to;

            return (T)this;
        }

        public PagedBinding ToPagedBinding()
        {
            return new PagedBinding(Page, PageSize);
        }
    }
}

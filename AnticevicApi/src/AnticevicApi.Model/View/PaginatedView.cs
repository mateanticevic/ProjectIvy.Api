using System.Collections.Generic;

namespace AnticevicApi.Model.View
{
    public class PaginatedView<T>
    {
        public PaginatedView()
        {

        }

        public PaginatedView(IEnumerable<T> items, long count)
        {
            Count = count;
            Items = items;
        }

        public long Count { get; set; }

        public IEnumerable<T> Items { get; set; }
    }
}

using System.Collections.Generic;

namespace AnticevicApi.Model.View
{
    public class PaginatedView<T>
    {
        public IEnumerable<T> Items { get; set; }

        public int Pages { get; set; }
    }
}

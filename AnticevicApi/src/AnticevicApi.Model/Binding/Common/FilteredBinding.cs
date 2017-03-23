using System;

namespace AnticevicApi.Model.Binding.Common
{
    public class FilteredBinding
    {
        public FilteredBinding()
        {
        }

        public FilteredBinding(DateTime? from, DateTime? to)
        {
            From = from;
            To = to;
        }

        public DateTime? From { get; set; }

        public DateTime? To { get; set; }

        public bool OrderAscending { get; set; }
    }
}

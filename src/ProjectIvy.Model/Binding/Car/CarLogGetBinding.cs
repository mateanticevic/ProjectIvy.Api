using System;

namespace ProjectIvy.Model.Binding.Car
{
    public class CarLogGetBinding : FilteredPagedBinding
    {
        public bool? HasOdometer { get; set; }
    }
}

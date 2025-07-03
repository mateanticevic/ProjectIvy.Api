using System;

namespace ProjectIvy.Model.Binding;

public interface IFilteredBinding
{
    DateTime? From { get; set; }
    DateTime? To { get; set; }
}

using System;
using ProjectIvy.Model.Binding.Route;

namespace ProjectIvy.Model.Binding.ToDo;

public class ToDoGetBinding : FilteredPagedBinding, ISearchable
{
    public bool? IsCompleted { get; set; }

    public DateTime? FromDueDate { get; set; }

    public DateTime? ToDueDate { get; set; }

    public IEnumerable<string> TagId { get; set; }

    public IEnumerable<string> TripId { get; set; }

    public string Search { get; set; }
}

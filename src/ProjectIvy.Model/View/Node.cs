using System.Collections.Generic;

namespace ProjectIvy.Model.View;

public class Node<T>
{
    public IEnumerable<Node<T>> Children { get; set; }

    public T This { get; set; }
}

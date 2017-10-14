using System.Collections.Generic;

namespace ProjectIvy.Model.View
{
    public class Node<T>
    {
        public T This { get; set; }

        public IEnumerable<Node<T>> Children { get; set; }
    }
}

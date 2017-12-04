namespace ProjectIvy.Model.Binding
{
    public interface IOrderable<T>
    {
        bool OrderAscending { get; set; }

        T OrderBy { get; set; }
    }
}

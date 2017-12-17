namespace ProjectIvy.Model.Binding
{
    public class PagedBinding : IPagedBinding
    {
        public bool PageAll { get; set; }

        public int Page { get; set; }

        public int PageSize { get; set; }
    }
}

namespace ProjectIvy.Model.Binding
{
    public class PagedBinding : IPagedBinding
    {
        public PagedBinding()
        {
            Page = 0;
            PageSize = 10;
        }

        public PagedBinding(int? page, int? pageSize)
        {
            Page = page ?? 0;
            PageSize = pageSize ?? 10;
        }

        public int Page { get; set; }

        public int PageSize { get; set; }
    }
}

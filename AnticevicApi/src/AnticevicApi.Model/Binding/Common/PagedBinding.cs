namespace AnticevicApi.Model.Binding.Common
{
    public class PagedBinding
    {
        public PagedBinding()
        {

        }

        public PagedBinding(int? page, int? pageSize)
        {
            Page = page.HasValue ? page : 0;
            PageSize = pageSize.HasValue ? pageSize : 10;
        }

        public int? Page { get; set; }
        public int? PageSize { get; set; }
    }
}

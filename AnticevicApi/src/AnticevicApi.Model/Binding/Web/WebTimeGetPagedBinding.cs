using AnticevicApi.Model.Binding.Common;

namespace AnticevicApi.Model.Binding.Web
{
    public class WebTimeGetPagedBinding : FilteredPagedBinding
    {
        public string DeviceId { get; set; }
    }
}

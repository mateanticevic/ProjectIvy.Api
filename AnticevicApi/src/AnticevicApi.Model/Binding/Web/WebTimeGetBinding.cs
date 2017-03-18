using AnticevicApi.Model.Binding.Common;

namespace AnticevicApi.Model.Binding.Web
{
    public class WebTimeGetBinding : FilteredBinding
    {
        public string DomainId { get; set; }
        public string DeviceId { get; set; }
        public bool? IsSecured { get; set; }
        public string Webid { get; set; }
    }
}

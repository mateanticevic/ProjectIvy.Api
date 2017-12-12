namespace ProjectIvy.Model.Binding.Log
{
    public class LogBrowserGetBinding : FilteredPagedBinding
    {
        public string DeviceId { get; set; }

        public string DomainId { get; set; }

        public bool? IsSecured { get; set; }

        public string WebId { get; set; }
    }
}

namespace ProjectIvy.Model.Binding.Web;

public class WebTimeGetBinding : FilteredBinding
{
    public string DomainId { get; set; }

    public string DeviceId { get; set; }

    public bool? IsSecured { get; set; }

    public string WebId { get; set; }
}

namespace ProjectIvy.Model.Binding.Web;

public class WebTimeGetBinding : FilteredBinding
{
    public string DeviceId { get; set; }

    public string DomainId { get; set; }

    public bool? IsSecured { get; set; }

    public string WebId { get; set; }
}

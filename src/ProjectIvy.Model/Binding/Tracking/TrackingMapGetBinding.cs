using System.ComponentModel.DataAnnotations;

namespace ProjectIvy.Model.Binding.Tracking;

public class TrackingMapGetBinding
{
    [Required]
    public DateOnly? Date { get; set; }

    public string Timezone { get; set; }
}

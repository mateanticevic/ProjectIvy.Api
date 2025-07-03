using System.ComponentModel.DataAnnotations.Schema;

namespace ProjectIvy.Model.Database.Main.Travel;

[Table(nameof(TripFile), Schema = nameof(Travel))]
public class TripFile
{
    public int TripId { get; set; }

    public int FileId { get; set; }

    public string Name { get; set; }

    public Storage.File File { get; set; }

    public Trip Trip { get; set; }
}

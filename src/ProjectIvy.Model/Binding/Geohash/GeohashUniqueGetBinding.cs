namespace ProjectIvy.Model.Binding.Geohash;

public class GeohashUniqueGetBinding : FilteredBinding
{
    public bool OnlyNew { get; set; }

    public int Precision { get; set; } = 8;
}
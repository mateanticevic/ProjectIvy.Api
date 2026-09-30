namespace ProjectIvy.Model.Binding.Geohash;

public class GeohashGetBinding : FilteredBinding
{
    public bool All { get; set; }

    public string Geohash { get; set; }

    public int Precision { get; set; } = 9;
}

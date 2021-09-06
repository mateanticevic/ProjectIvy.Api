namespace ProjectIvy.Model.Binding.Geohash
{
    public class GeohashGetBinding : FilteredBinding
    {
        public string Geohash { get; set; }

        public int Precision { get; set; } = 9;

        public bool All { get; set; }
    }
}

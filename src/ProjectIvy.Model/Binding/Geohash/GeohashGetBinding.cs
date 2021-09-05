namespace ProjectIvy.Model.Binding.Geohash
{
    public class GeohashGetBinding
    {
        public string Geohash { get; set; }

        public int Precision { get; set; } = 9;

        public bool All { get; set; }
    }
}

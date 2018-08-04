namespace ProjectIvy.Model.View.Beer
{
    public class BeerBrand
    {
        public BeerBrand(Database.Main.Beer.BeerBrand b)
        {
            Id = b.ValueId;
            Name = b.Name;
        }

        public string Id { get; set; }

        public string Name { get; set; }
    }
}

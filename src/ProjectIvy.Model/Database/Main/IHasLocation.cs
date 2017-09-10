namespace ProjectIvy.Model.Database.Main
{
    public interface IHasLocation
    {
        decimal Latitude { get; set; }

        decimal Longitude { get; set; }
    }
}

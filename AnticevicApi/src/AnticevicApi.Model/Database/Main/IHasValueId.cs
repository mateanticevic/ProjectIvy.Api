namespace AnticevicApi.Model.Database.Main
{
    public interface IHasValueId
    {
        int Id { get; set; }
        string ValueId { get; set; }
    }
}

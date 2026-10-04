namespace ProjectIvy.Model.View.Tracking;

public class TrackingView
{
    public TrackingView(Database.Main.Tracking.TrackingView entity)
    {
        Id = entity.ValueId;
        Name = entity.Name;
        From = entity.From;
        To = entity.To;
    }

    public string Id { get; set; }
    public string Name { get; set; }
    public DateTime From { get; set; }
    public DateTime To { get; set; }
}

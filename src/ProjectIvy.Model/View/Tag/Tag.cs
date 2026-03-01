namespace ProjectIvy.Model.View.Tag;

public class Tag
{
    public Tag(Database.Main.Common.Tag x)
    {
        Id = x.ValueId;
        Name = x.Name;
    }

    public string Id { get; set; }

    public string Name { get; set; }
}

using DatabaseModel = ProjectIvy.Model.Database.Main.Inventory;

namespace ProjectIvy.Model.View.Inventory;

public class Ownership
{
    public Ownership() { }

    public Ownership(DatabaseModel.Ownership x)
    {
        Id = x.ValueId;
        Name = x.Name;
    }

    public string Id { get; set; }

    public string Name { get; set; }
}

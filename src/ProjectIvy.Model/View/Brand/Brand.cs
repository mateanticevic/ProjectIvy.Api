using DatabaseModel = ProjectIvy.Model.Database.Main.Common;

namespace ProjectIvy.Model.View.Brand;

public class Brand
{
    public Brand() { }

    public Brand(DatabaseModel.Brand x)
    {
        Id = x.ValueId;
        Name = x.Name;
    }

    public string Id { get; set; }

    public string Name { get; set; }
}

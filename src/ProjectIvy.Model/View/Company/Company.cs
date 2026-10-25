using DatabaseModel = ProjectIvy.Model.Database.Main.Common;

namespace ProjectIvy.Model.View.Company;

public class Company
{
    public Company(DatabaseModel.Company entity)
    {
        Id = entity.ValueId;
        Name = entity.Name;
    }

    public string Id { get; set; }

    public string Name { get; set; }
}

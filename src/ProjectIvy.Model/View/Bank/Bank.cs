namespace ProjectIvy.Model.View.Bank;

public class Bank
{
    public Bank(Database.Main.Finance.Bank b)
    {
        Id = b.ValueId;
        Name = b.Name;
    }

    public string Id { get; set; }

    public string Name { get; set; }
}

namespace ProjectIvy.Model.View.Expense;

public class ExpenseFileType
{
    public ExpenseFileType(Database.Main.Finance.ExpenseFileType entity)
    {
        Id = entity.ValueId;
        Name = entity.Name;
    }

    public string Id { get; set; }

    public string Name { get; set; }
}

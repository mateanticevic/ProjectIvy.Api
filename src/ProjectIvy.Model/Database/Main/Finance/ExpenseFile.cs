using System.ComponentModel.DataAnnotations.Schema;

namespace ProjectIvy.Model.Database.Main.Finance;

[Table(nameof(ExpenseFile), Schema = nameof(Finance))]
public class ExpenseFile : IHasName
{
    public string Name { get; set; }

    public int ExpenseId { get; set; }

    public int FileId { get; set; }

    public int ExpenseFileTypeId { get; set; }

    public Expense Expense { get; set; }

    public ExpenseFileType ExpenseFileType { get; set; }

    public Storage.File File { get; set; }
}

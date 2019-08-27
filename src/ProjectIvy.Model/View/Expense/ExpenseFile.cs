using ProjectIvy.Common.Extensions;
using DatabaseModel = ProjectIvy.Model.Database.Main.Finance;

namespace ProjectIvy.Model.View.Expense
{
    public class ExpenseFile
    {
        public ExpenseFile(DatabaseModel.ExpenseFile entity)
        {
            Name = entity.Name;
            File = entity.File.ConvertTo(x => new File.File(x));
            Type = entity.ExpenseFileType.ConvertTo(x => new ExpenseFileType(x));
        }

        public string Name { get; set; }

        public File.File File { get; set; }

        public ExpenseFileType Type { get; set; }
    }
}

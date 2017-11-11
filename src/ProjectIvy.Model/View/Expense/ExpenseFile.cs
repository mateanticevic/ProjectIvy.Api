using DatabaseModel = ProjectIvy.Model.Database.Main.Finance;
using ProjectIvy.Extensions.BuiltInTypes;

namespace ProjectIvy.Model.View.Expense
{
    public class ExpenseFile
    {
        public ExpenseFile(DatabaseModel.ExpenseFile entity)
        {
            Name = entity.Name;
            File = entity.File.ConvertTo(x => new File.File(x));
        }

        public string Name { get; set; }

        public File.File File { get; set; }
    }
}

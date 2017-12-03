using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace ProjectIvy.Model.Database.Main.Finance
{
    [Table(nameof(ExpenseType), Schema = nameof(Finance))]
    public class ExpenseType : IHasValueId, IHasName
    {
        [Key]
        public int Id { get; set; }

        public string ValueId { get; set; }

        public string Name { get; set; }

        public string Icon { get; set; }

        public int? ParentTypeId { get; set; }

        public ICollection<ExpenseType> Children { get; set; }

        public ICollection<Expense> Expenses { get; set; }

        public ExpenseType ParentType { get; set; }
    }
}

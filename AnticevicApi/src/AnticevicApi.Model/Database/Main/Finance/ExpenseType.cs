﻿using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace AnticevicApi.Model.Database.Main.Finance
{
    [Table("ExpenseType", Schema = "Finance")]
    public class ExpenseType : IHasValueId
    {
        [Key]
        public int Id { get; set; }

        public string Icon { get; set; }

        public string IconUrl { get; set; }

        public string ValueId { get; set; }

        public string TypeDescription { get; set; }

        public int? ParentTypeId { get; set; }

        public ICollection<ExpenseType> Children { get; set; }

        public ICollection<Expense> Expenses { get; set; }

        public ExpenseType ParentType { get; set; }
    }
}

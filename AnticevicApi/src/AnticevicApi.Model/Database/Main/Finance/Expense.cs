using AnticevicApi.Model.Database.Main.Common;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System;

namespace AnticevicApi.Model.Database.Main.Finance
{
    [Table("Expense", Schema = "Finance")]
    public class Expense : UserEntity
    {
        public Expense()
        {
            Modified = DateTime.Now;
            ValueId = Guid.NewGuid().ToString().Substring(0, 8);
        }

        [Key]
        public int Id { get; set; }

        public decimal Ammount { get; set; }

        public string Comment { get; set; }

        public int CurrencyId { get; set; }

        public Currency Currency { get; set; }

        public DateTime Date { get; set; }

        public int ExpenseTypeId { get; set; }

        public ExpenseType ExpenseType { get; set; }

        public DateTime? Modified { get; set; }

        public string ValueId { get; set; }

        public int? VendorId { get; set; }

        public Vendor Vendor { get; set; }
    }
}

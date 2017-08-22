using AnticevicApi.Model.Database.Main.Common;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System;

namespace AnticevicApi.Model.Database.Main.Finance
{
    [Table(nameof(Expense), Schema = nameof(Finance))]
    public class Expense : UserEntity, IHasValueId
    {
        public Expense()
        {
            Modified = DateTime.Now;
        }

        [Key]
        public int Id { get; set; }

        public string ValueId { get; set; }

        public decimal Ammount { get; set; }

        public string Comment { get; set; }

        public int CurrencyId { get; set; }
        
        public DateTime Date { get; set; }

        public int ExpenseTypeId { get; set; }
        
        public DateTime? Modified { get; set; }

        public int? PoiId { get; set; }
        
        public int? VendorId { get; set; }

        public int? PaymentTypeId { get; set; }

        public int? CardId { get; set; }

        public Currency Currency { get; set; }

        public Card Card { get; set; }

        public ExpenseType ExpenseType { get; set; }

        public PaymentType PaymentType { get; set; }

        public Travel.Poi Poi { get; set; }

        public Vendor Vendor { get; set; }
    }
}

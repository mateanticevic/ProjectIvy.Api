using ProjectIvy.Model.Database.Main.Common;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ProjectIvy.Model.Database.Main.Finance
{
    [Table(nameof(Expense), Schema = nameof(Finance))]
    public class Expense : UserEntity, IHasValueId, IHasCreatedModified
    {
        public Expense()
        {
            Modified = DateTime.Now;
            Created = DateTime.Now;
        }

        [Key]
        public int Id { get; set; }

        public string ValueId { get; set; }

        public decimal Amount { get; set; }

        public string Comment { get; set; }

        public string InstallmentRef { get; set; }

        public int CurrencyId { get; set; }

        public int? ParentCurrencyId { get; set; }

        public decimal? ParentCurrencyExchangeRate { get; set; }

        public DateTime Date { get; set; }

        public int ExpenseTypeId { get; set; }

        public DateTime Modified { get; set; }

        public DateTime Created { get; set; }

        public int? PoiId { get; set; }

        public int? VendorId { get; set; }

        public int? PaymentTypeId { get; set; }

        public int? CardId { get; set; }

        public bool NeedsReview { get; set; }

        public Currency Currency { get; set; }

        public Currency ParentCurrency { get; set; }

        public Card Card { get; set; }

        public ExpenseType ExpenseType { get; set; }

        public PaymentType PaymentType { get; set; }

        public Travel.Poi Poi { get; set; }

        public Vendor Vendor { get; set; }

        public ICollection<ExpenseFile> ExpenseFiles { get; set; }
    }
}

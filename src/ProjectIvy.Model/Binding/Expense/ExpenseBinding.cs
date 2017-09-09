using System;

namespace ProjectIvy.Model.Binding.Expense
{
    public class ExpenseBinding
    {
        public decimal Amount { get; set; }

        public string Comment { get; set; }

        public string CurrencyId { get; set; }

        public DateTime Date { get; set; }

        public string ExpenseTypeId { get; set; }

        public string PaymentTypeId { get; set; }

        public string CardId { get; set; }

        public string PoiId { get; set; }

        public string Id { get; set; }

        public string VendorId { get; set; }
    }
}

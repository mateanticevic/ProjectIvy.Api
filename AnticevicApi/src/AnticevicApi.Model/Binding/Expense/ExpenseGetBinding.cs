﻿using AnticevicApi.Model.Binding.Common;

namespace AnticevicApi.Model.Binding.Expense
{
    public class ExpenseGetBinding : FilteredPagedBinding
    {
        public decimal? AmountFrom { get; set; }
        public decimal? AmountTo { get; set; }
        public string CurrencyId { get; set; }
        public string Description { get; set; }
        public string TypeId { get; set; }
        public string VendorId { get; set; }
    }
}

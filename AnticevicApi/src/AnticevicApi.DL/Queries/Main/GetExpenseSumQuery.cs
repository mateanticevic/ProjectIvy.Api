using System;

namespace AnticevicApi.DL.Queries.Main
{
    public class GetExpenseSumQuery
    {
        public DateTime? From { get; set; }
        public DateTime? To { get; set; }
        public int UserId { get; set; }
        public int TargetCurrencyId { get; set; }
        public string ExpenseTypeValueId { get; set; }
        public string VendorValueId { get; set; }
    }
}

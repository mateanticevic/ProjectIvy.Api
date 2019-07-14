using System;

namespace ProjectIvy.Model.Services.TrasnferWise
{
    public class Transfer
    {
        public long Id { get; set; }
        public long User { get; set; }
        public long TargetAccount { get; set; }
        public object SourceAccount { get; set; }
        public object Quote { get; set; }
        public Guid QuoteUuid { get; set; }
        public string Status { get; set; }
        public string Reference { get; set; }
        public long Rate { get; set; }
        public DateTimeOffset Created { get; set; }
        public object Business { get; set; }
        public object TransferRequest { get; set; }
        public bool HasActiveIssues { get; set; }
        public string SourceCurrency { get; set; }
        public long SourceValue { get; set; }
        public string TargetCurrency { get; set; }
        public long TargetValue { get; set; }
        public Guid CustomerTransactionId { get; set; }
    }
}

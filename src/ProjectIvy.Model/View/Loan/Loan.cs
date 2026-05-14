using System;
using ProjectIvy.Common.Extensions;

namespace ProjectIvy.Model.View.Loan;

public class Loan
{
    public Loan(Database.Main.Finance.Loan l)
    {
        Id = l.Id;
        PrincipalAmount = l.PrincipalAmount;
        InterestRate = l.InterestRate;
        StartDate = l.StartDate;
        NumberOfPayments = l.NumberOfPayments;
        DisbursmentDate = l.DisbursmentDate;
        FirstPaymentDate = l.FirstPaymentDate;
        RepaymentType = l.RepaymentType;
        Bank = l.Bank.ConvertTo(b => new Bank.Bank(b));
        Currency = l.Currency.ConvertTo(c => new Currency.Currency(c));
    }

    public int Id { get; set; }

    public decimal PrincipalAmount { get; set; }

    public decimal InterestRate { get; set; }

    public DateTime StartDate { get; set; }

    public int NumberOfPayments { get; set; }

    public DateTime DisbursmentDate { get; set; }

    public DateTime FirstPaymentDate { get; set; }

    public string RepaymentType { get; set; }

    public Bank.Bank Bank { get; set; }

    public Currency.Currency Currency { get; set; }
}

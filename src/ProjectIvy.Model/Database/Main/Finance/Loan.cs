using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using ProjectIvy.Model.Database.Main.Common;

namespace ProjectIvy.Model.Database.Main.Finance;

[Table(nameof(Loan), Schema = nameof(Finance))]
public class Loan : UserEntity
{
    [Key]
    public int Id { get; set; }

    public decimal PrincipalAmount { get; set; }

    public decimal InterestRate { get; set; }

    public int CurrencyId { get; set; }

    public DateTime StartDate { get; set; }

    public int NumberOfPayments { get; set; }

    public DateTime DisbursmentDate { get; set; }

    public DateTime FirstPaymentDate { get; set; }

    public int BankId { get; set; }

    public string RepaymentType { get; set; }

    public Bank Bank { get; set; }

    public Currency Currency { get; set; }

    public ICollection<LoanPayment> LoanPayments { get; set; }
}

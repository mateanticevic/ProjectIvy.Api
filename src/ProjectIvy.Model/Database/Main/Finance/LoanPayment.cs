using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ProjectIvy.Model.Database.Main.Finance;

[Table(nameof(LoanPayment), Schema = nameof(Finance))]
public class LoanPayment
{
    [Key]
    public int Id { get; set; }

    public int LoanId { get; set; }

    public DateTime DueDate { get; set; }

    public int PeriodNumber { get; set; }

    public DateTime? PaidDate { get; set; }

    public decimal ScheduledAmount { get; set; }

    public decimal? PaidAmount { get; set; }

    public decimal? PrincipalAmount { get; set; }

    public decimal? InterestAmount { get; set; }

    public decimal? RemainingBalance { get; set; }

    public Loan Loan { get; set; }
}

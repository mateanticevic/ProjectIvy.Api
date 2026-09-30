using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using ProjectIvy.Model.Database.Main.Common;

namespace ProjectIvy.Model.Database.Main.Finance;

[Table(nameof(Income), Schema = nameof(Finance))]
public class Income : UserEntity, IHasCreatedModified
{
    public decimal Amount { get; set; }

    public DateTime Created { get; set; }

    public Currency Currency { get; set; }

    public int CurrencyId { get; set; }

    public DateTime Date { get; set; }

    public string Description { get; set; }

    [Key]
    public int Id { get; set; }

    public IncomeSource IncomeSource { get; set; }

    public int IncomeSourceId { get; set; }

    public IncomeType IncomeType { get; set; }

    public int IncomeTypeId { get; set; }

    public DateTime Modified { get; set; }
}

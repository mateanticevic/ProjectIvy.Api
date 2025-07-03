using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ProjectIvy.Model.Database.Main.Transport;

[Table(nameof(CarFuel), Schema = nameof(Transport))]
public class CarFuel
{
    [Key]
    public int Id { get; set; }

    public int CarId { get; set; }

    public int? ExpenseId { get; set; }

    public decimal AmountInLiters { get; set; }

    public DateTime Timestamp { get; set; }

    public Car Car { get; set; }

    public Finance.Expense Expense { get; set; }
}

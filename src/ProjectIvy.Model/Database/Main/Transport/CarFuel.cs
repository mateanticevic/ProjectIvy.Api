using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ProjectIvy.Model.Database.Main.Transport;

[Table(nameof(CarFuel), Schema = nameof(Transport))]
public class CarFuel
{
    public decimal AmountInLiters { get; set; }

    public Car Car { get; set; }

    public int CarId { get; set; }

    public Finance.Expense Expense { get; set; }

    public int? ExpenseId { get; set; }

    [Key]
    public int Id { get; set; }

    public DateTime Timestamp { get; set; }
}

using System.ComponentModel.DataAnnotations;

namespace ProjectIvy.Model.Binding.Salary;

public class SalaryBinding
{
    [Required]
    public string CurrencyId { get; set; }

    public DateTime Date { get; set; }

    public decimal? Deductions { get; set; }

    public decimal Gross { get; set; }

    public decimal GrossTotal { get; set; }

    public decimal Medical { get; set; }

    public decimal Net { get; set; }

    public decimal NetTotal { get; set; }

    [MaxLength(1000)]
    public string Note { get; set; }

    public decimal Tax { get; set; }

    public decimal TravelExpenses { get; set; }
}

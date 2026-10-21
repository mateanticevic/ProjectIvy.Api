namespace ProjectIvy.Model.Binding.Salary;

public class SalaryGetBinding : FilteredPagedBinding
{
    public string CompanyId { get; set; }

    public string CurrencyId { get; set; }
}

using ProjectIvy.Common.Extensions;
using DatabaseModel = ProjectIvy.Model.Database.Main.Finance;

namespace ProjectIvy.Model.View.Salary;

public class Salary
{
    public Salary(DatabaseModel.Salary entity)
    {
        Currency = entity.Currency.ConvertTo(x => new Currency.Currency(x));
        Date = entity.Date;
        Deductions = entity.Deductions;
        Gross = entity.Gross;
        GrossTotal = entity.GrossTotal;
        Id = entity.ValueId;
        Medical = entity.Medical;
        Net = entity.Net;
        NetTotal = entity.NetTotal;
        Note = entity.Note;
        Tax = entity.Tax;
        TravelExpenses = entity.TravelExpenses;
    }

    public Currency.Currency Currency { get; set; }

    public DateTime Date { get; set; }

    public decimal? Deductions { get; set; }

    public decimal Gross { get; set; }

    public decimal GrossTotal { get; set; }

    public string Id { get; set; }

    public decimal Medical { get; set; }

    public decimal Net { get; set; }

    public decimal NetTotal { get; set; }

    public string Note { get; set; }

    public decimal Tax { get; set; }

    public decimal TravelExpenses { get; set; }
}

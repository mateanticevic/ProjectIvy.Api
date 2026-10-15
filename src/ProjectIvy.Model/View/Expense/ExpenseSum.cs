#nullable enable

using System.Collections.Generic;
using System.ComponentModel;
using System.Text.Json.Serialization;

namespace ProjectIvy.Model.View.Expense;

public class ExpenseSum
{
    [Description("Total expense amount when no grouping is selected.")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public decimal? Total { get; set; }

    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public IEnumerable<KeyValuePair<DateTime, decimal>>? ByDay { get; set; }

    [Description("Expense sums by weekday: 0 Monday through 6 Sunday.")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public IEnumerable<KeyValuePair<int, decimal>>? ByDayOfWeek { get; set; }

    [Description("Expense sums by calendar month across years: 1 through 12.")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public IEnumerable<KeyValuePair<int, decimal>>? ByMonth { get; set; }

    [Description("Expense sums by individual year/month period.")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public IEnumerable<KeyValuePair<string, decimal>>? ByMonthOfYear { get; set; }

    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public IEnumerable<KeyValuePair<int, decimal>>? ByYear { get; set; }

    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public IEnumerable<KeyValuePair<Currency.Currency, decimal>>? ByCurrency { get; set; }

    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public IEnumerable<KeyValuePair<string, decimal>>? ByType { get; set; }

    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public IEnumerable<KeyValuePair<string, IEnumerable<KeyValuePair<string, decimal>>>>? ByMonthOfYearByType { get; set; }

    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public IEnumerable<KeyValuePair<short, IEnumerable<KeyValuePair<string, decimal>>>>? ByYearByType { get; set; }
}

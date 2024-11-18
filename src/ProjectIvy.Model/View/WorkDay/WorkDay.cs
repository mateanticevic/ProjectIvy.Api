using System.Text.Json.Serialization;
using ProjectIvy.Model.Constants.Database;

namespace ProjectIvy.Model.View.WorkDay;

public class WorkDay
{
    [JsonConverter(typeof(DateFormatConverter))]
    public DateTime Date { get; set; }
    
    public WorkDayType? Type { get; set; }
}
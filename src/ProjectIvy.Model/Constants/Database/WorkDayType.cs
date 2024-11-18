using System.Text.Json.Serialization;

namespace ProjectIvy.Model.Constants.Database;

[JsonConverter(typeof(JsonStringEnumConverter))]
public enum WorkDayType
{
    Holiday = 0,
    Office = 1,
    Remote = 2,
    Vacation = 3,
    SickLeave = 4,
    Conference = 5,
    BusinessTrip = 6,
    MedicalCheckUp = 7,
}

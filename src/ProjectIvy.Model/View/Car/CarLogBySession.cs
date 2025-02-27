namespace ProjectIvy.Model.View.Car;

public class CarLogBySession
{
    public DateTime End { get; set; }

    public DateTime Start { get; set; }

    public int? Distance { get; set; }

    public int Count { get; set; }

    public decimal? FuelUsed { get; set; }

    public short? MaxEngineRpm { get; set; }

    public short? MaxSpeed { get; set; }

    public string Session { get; set; }
}

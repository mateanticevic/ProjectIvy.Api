using System;

namespace ProjectIvy.Model.View.Car;

public class CarLogBySession
{
    public int Count { get; set; }

    public int? Distance { get; set; }

    public DateTime End { get; set; }

    public decimal? FuelUsed { get; set; }

    public short? MaxEngineRpm { get; set; }

    public short? MaxSpeed { get; set; }

    public string Session { get; set; }

    public DateTime Start { get; set; }
}

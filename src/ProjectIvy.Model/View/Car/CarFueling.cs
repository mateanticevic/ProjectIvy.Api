namespace ProjectIvy.Model.View.Car;

public class CarFueling
{
	public CarFueling(Database.Main.Transport.CarFuel c)
	{
		AmountInLiters = c.AmountInLiters;
		Date = c.Timestamp;
	}

	public decimal AmountInLiters { get; set; }

	public DateTime Date { get; set; }
}

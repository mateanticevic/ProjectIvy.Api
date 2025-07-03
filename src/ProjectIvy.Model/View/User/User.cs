using DatabaseModel = ProjectIvy.Model.Database.Main;

namespace ProjectIvy.Model.View.User;

public class User
{
    public User(DatabaseModel.User.User x)
    {
        DefaultCar = new Car.Car(x.DefaultCar);
        DefaultCurrency = new Currency.Currency(x.DefaultCurrency);
        FirstName = x.FirstName;
        LastName = x.LastName;
        Email = x.Email;
        Username = x.Username;
    }

    public Car.Car DefaultCar { get; set; }

    public Currency.Currency DefaultCurrency { get; set; }

    public string FirstName { get; set; }

    public string LastName { get; set; }

    public string Email { get; set; }

    public string Username { get; set; }
}

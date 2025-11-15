namespace ProjectIvy.Model.Binding.Stay;

public record StayBinding(
    DateTime From,
    DateTime To,
    string CityId,
    string CountryId
);
using ProjectIvy.Data.Extensions;
using ProjectIvy.Model.Binding.Stay;
using ProjectIvy.Model.Database.Main.Travel;

namespace ProjectIvy.Business.MapExtensions;

public static class StayExtensions
{
    public static Stay ToEntity(this StayBinding binding, Stay stay = null)
    {
        stay = stay ?? new Stay();

        stay.Date = binding.Date;

        return stay;
    }
}
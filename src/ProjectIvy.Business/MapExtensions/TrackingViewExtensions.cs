using ProjectIvy.Business.Exceptions;
using ProjectIvy.Data.Extensions;
using ProjectIvy.Model.Binding.Tracking;
using ProjectIvy.Model.Database.Main.Tracking;

namespace ProjectIvy.Business.MapExtensions;

public static class TrackingViewExtensions
{
    public static TrackingView ToEntity(this TrackingViewBinding binding, TrackingView entity = null)
    {
        if (binding is null || string.IsNullOrWhiteSpace(binding.Name)
            || !binding.From.HasValue || !binding.To.HasValue || binding.From > binding.To)
            throw new InvalidRequestException("Name and a valid date range are required.");

        if (entity is null)
        {
            var valueId = binding.Name.ToValueId();
            if (string.IsNullOrWhiteSpace(valueId))
                throw new InvalidRequestException("Name must produce a non-empty ID.");
            entity = new TrackingView { ValueId = valueId };
        }

        entity.Name = binding.Name;
        entity.From = binding.From.Value;
        entity.To = binding.To.Value;
        return entity;
    }
}

using View = AnticevicApi.Model.View.Trip;

namespace AnticevicApi.BL.Handlers.Trip
{
    public interface ITripHandler
    {
        View.Trip GetSingle(string valueId);
    }
}
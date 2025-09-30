using System.Threading.Tasks;
using ProjectIvy.Model.Binding.Stay;
using ProjectIvy.Model.View;
using View = ProjectIvy.Model.View.Stay;

namespace ProjectIvy.Business.Handlers.Stay;

public interface IStayHandler
{
    Task AddStay(DateTime date, string cityId, string countryId);

    Task<PagedView<View.Stay>> GetStays(StayGetBinding binding);
}
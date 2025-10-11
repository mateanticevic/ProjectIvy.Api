using System.Threading.Tasks;
using ProjectIvy.Model.Binding.Stay;
using ProjectIvy.Model.View;
using View = ProjectIvy.Model.View.Stay;

namespace ProjectIvy.Business.Handlers.Stay;

public interface IStayHandler
{
    Task AddStay(StayBinding binding);

    Task<PagedView<View.Stay>> GetStays(StayGetBinding binding);

    Task Update(int id, StayBinding binding);
}
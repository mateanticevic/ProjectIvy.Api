using System.Threading.Tasks;

namespace ProjectIvy.Business.Handlers.Vendor
{
    public interface IVacationHandler : IHandler
    {
        Task CreateVacation(DateTime date);

        Task DeleteVacation(DateTime date);
    }
}

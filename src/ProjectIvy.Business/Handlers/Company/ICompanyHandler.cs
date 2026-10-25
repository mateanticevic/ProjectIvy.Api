using System.Threading.Tasks;
using ProjectIvy.Model.Binding.Company;
using ProjectIvy.Model.View;
using View = ProjectIvy.Model.View.Company;

namespace ProjectIvy.Business.Handlers.Company;

public interface ICompanyHandler : IHandler
{
    Task<PagedView<View.Company>> Get(CompanyGetBinding binding);
}

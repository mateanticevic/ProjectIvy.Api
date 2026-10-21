using System.Threading.Tasks;
using ProjectIvy.Model.Binding.Salary;
using ProjectIvy.Model.View;
using View = ProjectIvy.Model.View.Salary;

namespace ProjectIvy.Business.Handlers.Salary;

public interface ISalaryHandler : IHandler
{
    Task<View.Salary> Create(SalaryBinding binding);

    Task Delete(string id);

    Task<PagedView<View.Salary>> Get(SalaryGetBinding binding);

    Task<View.Salary> GetSingle(string id);

    Task Update(string id, SalaryBinding binding);
}

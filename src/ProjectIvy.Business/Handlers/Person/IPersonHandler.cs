using System.Collections.Generic;
using System.Threading.Tasks;
using ProjectIvy.Model.Binding.Person;
using ProjectIvy.Model.View;
using View = ProjectIvy.Model.View.Person;

namespace ProjectIvy.Business.Handlers.Person;

public interface IPersonHandler
{
    Task<PagedView<View.Person>> Get(PersonGetBinding binding);

    Task<IEnumerable<KeyValuePair<DateTime, IEnumerable<View.Person>>>> GetByDateOfBirth();
}

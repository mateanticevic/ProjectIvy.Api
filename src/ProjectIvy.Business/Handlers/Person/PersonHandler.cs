using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using ProjectIvy.Data.Extensions;
using ProjectIvy.Model.Binding.Person;
using ProjectIvy.Model.View;
using View = ProjectIvy.Model.View.Person;

namespace ProjectIvy.Business.Handlers.Person;

public class PersonHandler : Handler<PersonHandler>, IPersonHandler
{
    public PersonHandler(IHandlerContext<PersonHandler> context) : base(context)
    {
    }

    public async Task<PagedView<View.Person>> Get(PersonGetBinding binding)
    {
        using var context = GetMainContext();
        var query = context.People
                           .Where(x => !x.IsDeleted);

        if (!string.IsNullOrEmpty(binding.Search))
        {
            var searchLower = binding.Search.ToLower();
            query = query.Where(x => 
                x.FirstName.ToLower().Contains(searchLower) ||
                x.LastName.ToLower().Contains(searchLower) ||
                x.ValueId.ToLower().Contains(searchLower));
        }

        return await query.OrderBy(x => x.FirstName)
                     .ThenBy(x => x.LastName)
                     .Select(x => new View.Person(x))
                     .ToPagedViewAsync(binding);
    }

    public async Task<IEnumerable<KeyValuePair<DateTime, IEnumerable<View.Person>>>> GetByDateOfBirth()
    {
        using var context = GetMainContext();
        
        var people = await context.People
                                  .Where(x => !x.IsDeleted)
                                  .OrderBy(x => x.DateOfBirth)
                                  .ThenBy(x => x.FirstName)
                                  .ThenBy(x => x.LastName)
                                  .ToListAsync();

        return people.GroupBy(x => x.DateOfBirth)
                     .Select(g => new KeyValuePair<DateTime, IEnumerable<View.Person>>(
                         g.Key,
                         g.Select(p => new View.Person(p))
                     ))
                     .ToList();
    }
}

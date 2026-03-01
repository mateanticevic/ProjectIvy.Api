using System.Linq;
using System.Threading.Tasks;
using ProjectIvy.Data.Extensions;
using ProjectIvy.Model.Binding.Tag;
using ProjectIvy.Model.View;
using View = ProjectIvy.Model.View.Tag;

namespace ProjectIvy.Business.Handlers.Tag;

public class TagHandler : Handler<TagHandler>, ITagHandler
{
    public TagHandler(IHandlerContext<TagHandler> context) : base(context)
    {
    }

    public async Task<PagedView<View.Tag>> Get(TagGetBinding binding)
    {
        using var context = GetMainContext();
        var query = context.Tags
                           .WhereUser(UserId)
                           .WhereSearch(binding)
                           .OrderByDescending(x => x.ValueId == binding.Search)
                           .ThenBy(x => x.Name)
                           .Select(x => new View.Tag(x));

        return await query.ToPagedViewAsync(binding);
    }

    public async Task<View.Tag> Create(TagBinding binding)
    {
        using var context = GetMainContext();

        var entity = new Model.Database.Main.Common.Tag
        {
            Name = binding.Name,
            UserId = UserId,
            ValueId = binding.Name.ToValueId()
        };

        context.Tags.Add(entity);
        await context.SaveChangesAsync();

        return new View.Tag(entity);
    }
}

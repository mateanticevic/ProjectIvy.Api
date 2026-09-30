using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using ProjectIvy.Business.Exceptions;
using ProjectIvy.Data.Extensions;
using ProjectIvy.Model.Binding.ExpenseType;
using ProjectIvy.Model.View;
using ProjectIvy.Model.View.Expense;
using ProjectIvy.Model.View.ExpenseType;
using Database = ProjectIvy.Model.Database.Main.Finance;

namespace ProjectIvy.Business.Handlers.Expense;

public class ExpenseTypeHandler : Handler<ExpenseTypeHandler>, IExpenseTypeHandler
{
    public ExpenseTypeHandler(IHandlerContext<ExpenseTypeHandler> context) : base(context)
    {
    }

    public async Task<ExpenseType> Create(ExpenseTypeBinding binding)
    {
        using var context = GetMainContext();
        
        int? parentTypeId = null;
        if (!string.IsNullOrWhiteSpace(binding.ParentId))
        {
            var parentType = await context.ExpenseTypes.FirstOrDefaultAsync(x => x.ValueId == binding.ParentId);
            if (parentType == null)
                throw new ResourceNotFoundException();
            parentTypeId = parentType.Id;
        }

        var expenseType = new Database.ExpenseType
        {
            Name = binding.Name,
            ParentTypeId = parentTypeId,
            ValueId = binding.Name.ToValueId()
        };

        await context.ExpenseTypes.AddAsync(expenseType);
        await context.SaveChangesAsync();

        return new ExpenseType(expenseType);
    }

    public async Task<IEnumerable<ExpenseType>> Get(ExpenseTypeGetBinding binding)
    {
        using var context = GetMainContext();
        int? parentId = await context.ExpenseTypes.GetIdAsync(binding.ParentId);

        var query = context.ExpenseTypes.Include(x => x.Children)
                                        .WhereIf(binding.HasChildren.HasValue, x => binding.HasChildren.Value ? x.Children.Any() : !x.Children.Any())
                                        .WhereIf(binding.HasParent.HasValue, x => binding.HasParent.Value ? x.ParentTypeId != null : x.ParentTypeId == null)
                                        .WhereIf(parentId.HasValue, x => x.ParentTypeId == parentId.Value);

        switch (binding.OrderBy)
        {
            case ExpenseTypeSort.Top10:
                var top10Types = context.Expenses.OrderByDescending(x => x.Created)
                                                 .Take(1000)
                                                 .GroupBy(x => x.ExpenseTypeId)
                                                 .Select(x => new { x.Key, Count = x.Count() })
                                                 .OrderByDescending(x => x.Count)
                                                 .Take(10);

                query = query.OrderBy(x => !top10Types.Any(y => y.Key == x.Id))
                             .ThenBy(x => x.Name);

                break;
            default:
                query = query.OrderBy(x => x.Name);
                break;
        }

        return await query.Select(x => new ExpenseType(x)).ToListAsync();
    }

    private IEnumerable<Node<ExpenseType>> GetChildrenNodes(IEnumerable<Database.ExpenseType> entities, int parentId)
    {
        var children = entities.Where(x => x.ParentTypeId == parentId).ToList();

        return children.Any() ? children.Select(x => new Node<ExpenseType>() { This = new ExpenseType(x), Children = GetChildrenNodes(entities, x.Id) }).ToList() : null;
    }

    public async Task<IEnumerable<ExpenseFileType>> GetFileTypes()
    {
        using var context = GetMainContext();
        return await context.ExpenseFileTypes.Select(x => new ExpenseFileType(x))
                                       .ToListAsync();
    }

    public async Task<IEnumerable<Node<ExpenseType>>> GetTree()
    {
        using var context = GetMainContext();
        var typeEntities = await context.ExpenseTypes.ToListAsync();

        var rootTypes = typeEntities.Where(x => !x.ParentTypeId.HasValue)
                                    .ToList();

        return rootTypes.Select(type => new Node<ExpenseType>() { This = new ExpenseType(type), Children = GetChildrenNodes(typeEntities, type.Id) }).ToList();
    }

    public async Task SetParent(string parentValueId, string childValueId)
    {
        using var context = GetMainContext();
        var parentType = await context.ExpenseTypes.FirstOrDefaultAsync(x => x.ValueId == parentValueId) ?? throw new ResourceNotFoundException();
        var childType = await context.ExpenseTypes.FirstOrDefaultAsync(x => x.ValueId == childValueId) ?? throw new ResourceNotFoundException();
        childType.ParentTypeId = parentType.Id;
        await context.SaveChangesAsync();
    }
}

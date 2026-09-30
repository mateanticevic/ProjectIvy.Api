using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using View = ProjectIvy.Model.View.PaymentType;

namespace ProjectIvy.Business.Handlers.PaymentType;

public class PaymentTypeHandler : Handler<PaymentTypeHandler>, IPaymentTypeHandler
{
    public PaymentTypeHandler(IHandlerContext<PaymentTypeHandler> context) : base(context)
    {
    }

    public async Task<IEnumerable<View.PaymentType>> GetPaymentTypes()
    {
        using var context = GetMainContext();
        return await context.PaymentTypes.Select(x => new View.PaymentType(x))
                                   .ToListAsync();
    }
}

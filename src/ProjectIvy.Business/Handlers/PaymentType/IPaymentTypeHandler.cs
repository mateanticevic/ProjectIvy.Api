using System.Collections.Generic;
using System.Threading.Tasks;
using View = ProjectIvy.Model.View.PaymentType;

namespace ProjectIvy.Business.Handlers.PaymentType;

public interface IPaymentTypeHandler : IHandler
{
    Task<IEnumerable<View.PaymentType>> GetPaymentTypes();
}

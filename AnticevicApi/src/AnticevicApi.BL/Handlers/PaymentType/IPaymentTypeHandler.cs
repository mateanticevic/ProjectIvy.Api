using System.Collections.Generic;
using View = AnticevicApi.Model.View.PaymentType;

namespace AnticevicApi.BL.Handlers.PaymentType
{
    public interface IPaymentTypeHandler : IHandler
    {
        IEnumerable<View.PaymentType> GetPaymentTypes();
    }
}

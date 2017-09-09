using System.Collections.Generic;
using View = ProjectIvy.Model.View.PaymentType;

namespace ProjectIvy.BL.Handlers.PaymentType
{
    public interface IPaymentTypeHandler : IHandler
    {
        IEnumerable<View.PaymentType> GetPaymentTypes();
    }
}

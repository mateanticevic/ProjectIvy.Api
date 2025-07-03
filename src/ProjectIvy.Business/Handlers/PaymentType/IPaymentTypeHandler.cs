using System.Collections.Generic;
using View = ProjectIvy.Model.View.PaymentType;

namespace ProjectIvy.Business.Handlers.PaymentType;

public interface IPaymentTypeHandler : IHandler
{
    IEnumerable<View.PaymentType> GetPaymentTypes();
}

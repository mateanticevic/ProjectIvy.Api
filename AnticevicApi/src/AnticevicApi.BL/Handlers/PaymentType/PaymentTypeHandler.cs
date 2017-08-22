﻿using System.Collections.Generic;
using System.Linq;
using View = AnticevicApi.Model.View.PaymentType;

namespace AnticevicApi.BL.Handlers.PaymentType
{
    public class PaymentTypeHandler : Handler<PaymentTypeHandler>, IPaymentTypeHandler
    {
        public PaymentTypeHandler(IHandlerContext<PaymentTypeHandler> context) : base(context)
        {
        }

        public IEnumerable<View.PaymentType> GetPaymentTypes()
        {
            using (var context = GetMainContext())
            {
                return context.PaymentTypes.Select(x => new View.PaymentType(x))
                                           .ToList();
            }
        }
    }
}

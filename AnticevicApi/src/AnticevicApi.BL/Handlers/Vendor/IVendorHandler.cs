using System.Collections.Generic;
using View = AnticevicApi.Model.View.Vendor;

namespace AnticevicApi.BL.Handlers.Vendor
{
    public interface IVendorHandler : IHandler
    {
        IEnumerable<View.Vendor> Get(string contains);
    }
}

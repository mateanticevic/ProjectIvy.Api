using System.Collections.Generic;
using View = ProjectIvy.Model.View.Vendor;

namespace ProjectIvy.BL.Handlers.Vendor
{
    public interface IVendorHandler : IHandler
    {
        IEnumerable<View.Vendor> Get(string contains);
    }
}

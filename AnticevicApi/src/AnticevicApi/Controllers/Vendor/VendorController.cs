using AnticevicApi.BL.Handlers.Vendor;
using AnticevicApi.Model.Constants;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System.Collections.Generic;
using ViewVendor = AnticevicApi.Model.View.Vendor;

namespace AnticevicApi.Controllers.Vendor
{
    [Route("[controller]")]
    public class VendorController : BaseController<VendorController>
    {
        private readonly IVendorHandler _vendorHandler;

        public VendorController(ILogger<VendorController> logger, IVendorHandler vendorHandler) : base(logger)
        {
            _vendorHandler = vendorHandler;
        }

        #region Get

        [HttpGet]
        public IEnumerable<ViewVendor.Vendor> Get([FromQuery] string contains)
        {
            Logger.LogInformation((int)LogEvent.ActionCalled, nameof(Get), contains);

            return _vendorHandler.Get(contains);
        }

        #endregion
    }
}

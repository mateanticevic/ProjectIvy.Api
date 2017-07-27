using AnticevicApi.BL.Handlers.Poi;
using AnticevicApi.BL.Handlers.Vendor;
using AnticevicApi.Model.Binding.Poi;
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
        private readonly IPoiHandler _poiHandler;
        private readonly IVendorHandler _vendorHandler;

        public VendorController(ILogger<VendorController> logger, IPoiHandler poiHandler, IVendorHandler vendorHandler) : base(logger)
        {
            _poiHandler = poiHandler;
            _vendorHandler = vendorHandler;
        }

        #region Get

        [HttpGet]
        public IEnumerable<ViewVendor.Vendor> Get([FromQuery] string contains)
        {
            Logger.LogInformation((int)LogEvent.ActionCalled, nameof(Get), contains);

            return _vendorHandler.Get(contains);
        }

        [HttpGet]
        [Route("{vendorId}/poi")]
        public IEnumerable<object> GetPois(string vendorId)
        {
            return _poiHandler.Get(new PoiGetBinding() { VendorId = vendorId });
        }

        #endregion
    }
}

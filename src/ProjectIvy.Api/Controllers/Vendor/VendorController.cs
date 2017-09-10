using ProjectIvy.BL.Handlers.Poi;
using ProjectIvy.BL.Handlers.Vendor;
using ProjectIvy.Model.Binding.Poi;
using ProjectIvy.Model.Constants;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System.Collections.Generic;
using ViewVendor = ProjectIvy.Model.View.Vendor;

namespace ProjectIvy.Api.Controllers.Vendor
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
            return _poiHandler.Get(new PoiGetBinding() { VendorId = vendorId, PageSize = null }).Items;
        }

        #endregion
    }
}

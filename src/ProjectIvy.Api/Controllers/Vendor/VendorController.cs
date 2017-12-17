using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using ProjectIvy.BL.Handlers.Poi;
using ProjectIvy.BL.Handlers.Vendor;
using ProjectIvy.Model.Binding.Poi;
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
        public IEnumerable<ViewVendor.Vendor> Get([FromQuery] string contains) => _vendorHandler.Get(contains);

        [HttpGet("{vendorId}/Poi")]
        public IEnumerable<object> GetPois(string vendorId) => _poiHandler.Get(new PoiGetBinding() { VendorId = vendorId, PageAll = true}).Items;

        #endregion
    }
}

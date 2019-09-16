using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using ProjectIvy.Business.Handlers.Poi;
using ProjectIvy.Business.Handlers.Vendor;
using ProjectIvy.Model.Binding.Poi;
using ProjectIvy.Model.Binding.Vendor;
using ProjectIvy.Model.View;
using System.Collections.Generic;
using ViewVendor = ProjectIvy.Model.View.Vendor;

namespace ProjectIvy.Api.Controllers.Vendor
{
    public class VendorController : BaseController<VendorController>
    {
        private readonly IPoiHandler _poiHandler;
        private readonly IVendorHandler _vendorHandler;

        public VendorController(ILogger<VendorController> logger, IPoiHandler poiHandler, IVendorHandler vendorHandler) : base(logger)
        {
            _poiHandler = poiHandler;
            _vendorHandler = vendorHandler;
        }

        [HttpGet("{id}")]
        public ViewVendor.Vendor Get(string id) => _vendorHandler.Get(id);

        [HttpGet]
        public PagedView<ViewVendor.Vendor> Get([FromQuery] VendorGetBinding binding) => _vendorHandler.Get(binding);

        [HttpGet("{vendorId}/Poi")]
        public IEnumerable<object> GetPois(string vendorId) => _poiHandler.Get(new PoiGetBinding() { VendorId = vendorId, PageAll = true }).Items;
    }
}

using AnticevicApi.BL.Handlers.Trip;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using View = AnticevicApi.Model.View.Trip;

namespace AnticevicApi.Controllers.Trip
{
    [Route("[controller]")]
    public class TripController : BaseController<TripController>
    {
        private readonly ITripHandler _tripHandler;

        public TripController(ILogger<TripController> logger, ITripHandler tripHandler) : base(logger)
        {
            _tripHandler = tripHandler;
        }

        #region Get

        [HttpGet]
        [Route("{id}")]
        public View.Trip Get(string id)
        {
            return _tripHandler.GetSingle(id);
        }

        #endregion
    }
}

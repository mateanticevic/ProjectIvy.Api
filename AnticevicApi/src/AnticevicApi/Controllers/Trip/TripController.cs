using AnticevicApi.BL.Handlers.Trip;
using AnticevicApi.Model.Binding.Trip;
using AnticevicApi.Model.View;
using Microsoft.AspNetCore.Http;
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

        [HttpDelete]
        [Route("{tripId}")]
        public StatusCodeResult Delete(string tripId)
        {
            _tripHandler.Delete(tripId);

            return new StatusCodeResult(StatusCodes.Status200OK);
        }

        #region Get

        [HttpGet]
        public PaginatedView<View.Trip> Get(TripGetBinding binding)
        {
            return _tripHandler.Get(binding);
        }

        [HttpGet]
        [Route("{id}")]
        public View.Trip Get(string id)
        {
            return _tripHandler.GetSingle(id);
        }

        #endregion

        [HttpPost]
        [Route("{tripId}/city/{cityId}")]
        public StatusCodeResult PostCity(string tripId, string cityId)
        {
            _tripHandler.AddCityToTrip(tripId, cityId);

            return new StatusCodeResult(StatusCodes.Status201Created);
        }

        [HttpPost]
        [Route("{tripId}")]
        public StatusCodeResult Post(string tripId, [FromBody] TripBinding binding)
        {
            binding.Id = tripId;
            _tripHandler.Create(binding);

            return new StatusCodeResult(StatusCodes.Status201Created);
        }
    }
}

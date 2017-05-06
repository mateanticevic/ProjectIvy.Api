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

        [HttpDelete]
        [Route("{tripId}/city/{cityId}")]
        public StatusCodeResult DeleteCity(string tripId, string cityId)
        {
            _tripHandler.RemoveCity(tripId, cityId);

            return new StatusCodeResult(StatusCodes.Status200OK);
        }

        [HttpDelete]
        [Route("{tripId}/expense/{expenseId}")]
        public StatusCodeResult DeleteExpense(string tripId, string expenseId)
        {
            _tripHandler.RemoveExpense(tripId, expenseId);

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
            _tripHandler.AddCity(tripId, cityId);

            return new StatusCodeResult(StatusCodes.Status201Created);
        }

        [HttpPost]
        [Route("{tripId}/expense/{expenseId}")]
        public StatusCodeResult PostExpense(string tripId, string expenseId)
        {
            _tripHandler.AddExpense(tripId, expenseId);

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

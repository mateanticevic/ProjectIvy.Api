using AnticevicApi.BL.Handlers.Card;
using AnticevicApi.Model.Constants.Database;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

namespace AnticevicApi.Controllers.Card
{
    [Authorize(Roles = UserRole.User)]
    [Route("[controller]")]
    public class CardController : BaseController<CardController>
    {
        private readonly ICardHandler _cardHandler;

        public CardController(ILogger<CardController> logger, ICardHandler cardHandler) : base(logger)
        {
            _cardHandler = cardHandler;
        }

        [HttpGet]
        public IActionResult Get()
        {
            return Ok(_cardHandler.GetCards());
        }
    }
}

using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using ProjectIvy.Business.Handlers.Card;
using ProjectIvy.Model.Binding.Card;
using ProjectIvy.Model.Constants.Database;

namespace ProjectIvy.Api.Controllers.Card
{
    [Authorize(Roles = UserRole.User)]
    public class CardController : BaseController<CardController>
    {
        private readonly ICardHandler _cardHandler;

        public CardController(ILogger<CardController> logger, ICardHandler cardHandler) : base(logger)
        {
            _cardHandler = cardHandler;
        }

        [HttpGet]
        public IActionResult Get([FromQuery] CardGetBinding binding) => Ok(_cardHandler.GetCards(binding));
    }
}

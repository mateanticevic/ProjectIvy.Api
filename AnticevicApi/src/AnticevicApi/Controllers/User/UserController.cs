using AnticevicApi.BL.Handlers.User;
using AnticevicApi.Model.Binding.User;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using View = AnticevicApi.Model.View.User;

namespace AnticevicApi.Controllers.User
{
    [Route("[controller]")]
    public class UserController : BaseController<UserController>
    {
        private readonly IUserHandler _userHandler;

        public UserController(ILogger<UserController> logger, IUserHandler userHandler) : base(logger)
        {
            _userHandler = userHandler;
        }

        [HttpGet]
        public View.User Get()
        {
            return _userHandler.Get();
        }

        [HttpPost]
        [Route("password")]
        public StatusCodeResult PostPassword([FromBody] PasswordSetBinding binding)
        {
            _userHandler.SetPassword(binding);

            return new StatusCodeResult(StatusCodes.Status200OK);
        }
    }
}

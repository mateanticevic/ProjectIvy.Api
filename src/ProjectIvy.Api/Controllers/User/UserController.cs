using ProjectIvy.BL.Handlers.User;
using ProjectIvy.Model.Binding.User;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using View = ProjectIvy.Model.View.User;

namespace ProjectIvy.Api.Controllers.User
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

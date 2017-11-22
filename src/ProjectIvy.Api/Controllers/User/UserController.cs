using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using ProjectIvy.BL.Handlers.User;
using ProjectIvy.Model.Binding.User;
using ProjectIvy.Model.Constants.Database;
using View = ProjectIvy.Model.View.User;

namespace ProjectIvy.Api.Controllers.User
{
    [Authorize(Roles = UserRole.User)]
    [Route("[controller]")]
    public class UserController : BaseController<UserController>
    {
        private readonly IUserHandler _userHandler;

        public UserController(ILogger<UserController> logger, IUserHandler userHandler) : base(logger) => _userHandler = userHandler;

        [HttpGet]
        public View.User Get()
        {
            return _userHandler.Get();
        }

        [HttpPost("Password")]
        public StatusCodeResult PostPassword([FromBody] PasswordSetBinding binding)
        {
            _userHandler.SetPassword(binding);

            return new StatusCodeResult(StatusCodes.Status200OK);
        }
    }
}

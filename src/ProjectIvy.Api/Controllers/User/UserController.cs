using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using ProjectIvy.Business.Handlers.User;
using ProjectIvy.Model.Binding.User;
using ProjectIvy.Model.Constants.Database;
using View = ProjectIvy.Model.View.User;

namespace ProjectIvy.Api.Controllers.User
{
    [Authorize(Roles = UserRole.User)]
    public class UserController : BaseController<UserController>
    {
        private readonly IUserHandler _userHandler;

        public UserController(ILogger<UserController> logger, IUserHandler userHandler) : base(logger) => _userHandler = userHandler;

        [HttpDelete("Session/{id:long}")]
        public async Task DeleteSession(long id) => await _userHandler.CloseSession(id);

        [HttpGet]
        public View.User Get() => _userHandler.Get();

        [HttpGet("Session")]
        public async Task<IActionResult> GetSessions() => Ok(await _userHandler.GetSessions());

        [HttpPost("Password")]
        public StatusCodeResult PostPassword([FromBody] PasswordSetBinding binding)
        {
            _userHandler.SetPassword(binding);

            return new StatusCodeResult(StatusCodes.Status200OK);
        }
    }
}

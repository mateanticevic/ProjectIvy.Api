using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using ProjectIvy.Business.Handlers.User;
using ProjectIvy.Model.Binding.User;
using View = ProjectIvy.Model.View.User;

namespace ProjectIvy.Api.Controllers.User
{
    public class UserController : BaseController<UserController>
    {
        private readonly IUserHandler _userHandler;

        public UserController(ILogger<UserController> logger, IUserHandler userHandler) : base(logger) => _userHandler = userHandler;

        [HttpGet]
        [ResponseCache(Duration = 10)]
        public View.User Get() => _userHandler.Get();

        [HttpPut]
        public async Task<IActionResult> Put([FromBody] UserUpdateBinding binding)
        {
            await _userHandler.Update(binding);
            return Ok();
        }
    }
}

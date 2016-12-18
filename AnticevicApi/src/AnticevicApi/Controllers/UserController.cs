using AnticevicApi.BL.Handlers.User;
using AnticevicApi.Model.View.User;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

namespace AnticevicApi.Controllers
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
        public User Get()
        {
            return _userHandler.Get();
        }
    }
}

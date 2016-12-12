using AnticevicApi.BL.Handlers.User;
using AnticevicApi.Config;
using AnticevicApi.Model.View.User;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace AnticevicApi.Controllers
{
    [Route("[controller]")]
    public class UserController : BaseController<UserController>
    {
        public UserController(IOptions<AppSettings> options, ILogger<UserController> logger, IUserHandler userHandler) : base(options, logger)
        {
            UserHandler = userHandler;
        }

        [HttpGet]
        public User Get()
        {
            return UserHandler.Get();
        }
    }
}

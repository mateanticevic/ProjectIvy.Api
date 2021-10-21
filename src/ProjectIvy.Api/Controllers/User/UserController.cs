using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using ProjectIvy.Business.Handlers.User;
using View = ProjectIvy.Model.View.User;

namespace ProjectIvy.Api.Controllers.User
{
    public class UserController : BaseController<UserController>
    {
        private readonly IUserHandler _userHandler;

        public UserController(ILogger<UserController> logger, IUserHandler userHandler) : base(logger) => _userHandler = userHandler;

        [HttpGet]
        public View.User Get() => _userHandler.Get();
    }
}

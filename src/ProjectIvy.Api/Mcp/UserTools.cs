using System.ComponentModel;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using ModelContextProtocol.Server;
using ProjectIvy.Api.Constants;
using ProjectIvy.Business.Handlers.User;
using View = ProjectIvy.Model.View.User;

namespace ProjectIvy.Api.Mcp;

[McpServerToolType]
public class UserTools
{
    private readonly IUserHandler _userHandler;

    public UserTools(IUserHandler userHandler)
    {
        _userHandler = userHandler;
    }

    [Authorize(ApiScopes.BasicUser)]
    [McpServerTool(Name = "get_current_user", ReadOnly = true, UseStructuredContent = true)]
    [Description("Returns the authenticated user's profile: username, first name, last name, email, tracking start date, default currency (code, name, symbol), and default car with its model details. Use this to identify the current user and retrieve their default settings, such as the currency used when recording expenses. Takes no arguments.")]
    public Task<View.User> GetCurrentUser() => _userHandler.Get();
}

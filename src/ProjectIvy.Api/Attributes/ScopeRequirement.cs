using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;

namespace ProjectIvy.Api.Attributes;

public class ScopeRequirement : IAuthorizationRequirement
{
    public ScopeRequirement(string requiredScope)
    {
        RequiredScope = requiredScope;
    }

    public string RequiredScope { get; }
}

public class ScopeRequirementHandler : AuthorizationHandler<ScopeRequirement>
{
    protected override Task HandleRequirementAsync(AuthorizationHandlerContext context, ScopeRequirement requirement)
    {
        var scopes = context.User.FindAll("scope").SelectMany(c => c.Value.Split(' ', System.StringSplitOptions.RemoveEmptyEntries));
        if (context.User.Identity?.IsAuthenticated == true && scopes.Contains(requirement.RequiredScope))
        {
            context.Succeed(requirement);
        }

        return Task.CompletedTask;
    }
}
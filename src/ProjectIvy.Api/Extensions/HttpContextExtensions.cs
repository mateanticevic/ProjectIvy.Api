using Microsoft.AspNetCore.Http;
using System.Linq;
using System.Net.Http.Headers;

namespace ProjectIvy.Api.Extensions
{
    public static class HttpContextExtensions
    {
        public static string GetAuthorizationToken(this HttpContext context)
        {
            string authorizationHeader = context.Request.Headers.SingleOrDefault(x => x.Key == "Authorization").Value;

            if (string.IsNullOrEmpty(authorizationHeader))
                authorizationHeader = context.Request.Cookies["Token"];

            return AuthenticationHeaderValue.TryParse(authorizationHeader, out var authorizationHeaderValue) ? authorizationHeaderValue.Parameter : authorizationHeader;
        }
    }
}

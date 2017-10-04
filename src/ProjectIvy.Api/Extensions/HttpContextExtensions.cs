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

            if (AuthenticationHeaderValue.TryParse(authorizationHeader, out var authorizationHeaderValue))
            {
                return authorizationHeaderValue.Parameter;
            }
            else
            {
                return authorizationHeader;
            }
        }
    }
}

using Microsoft.AspNetCore.Http;
using System.Linq;

namespace AnticevicApi.Extensions
{
    public static class HttpContextExtensions
    {
        public static string GetAuthorizationToken(this HttpContext context)
        {
            return context.Request.Headers.SingleOrDefault(x => x.Key == "Authorization").Value;
        }
    }
}

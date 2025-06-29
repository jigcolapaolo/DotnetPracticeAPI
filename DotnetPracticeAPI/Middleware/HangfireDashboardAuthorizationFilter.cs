using Hangfire.Dashboard;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;

namespace Middleware
{
    public class HangfireDashboardAuthorizationFilter : IDashboardAuthorizationFilter
    {
        public bool Authorize(DashboardContext context)
        {
            var httpContext = context.GetHttpContext();

            // Con Cookie
            var authResult = httpContext.AuthenticateAsync(CookieAuthenticationDefaults.AuthenticationScheme).Result;

            return authResult.Succeeded &&
                   authResult.Principal?.Identity?.IsAuthenticated == true &&
                   authResult.Principal.IsInRole("Admin");

            // Con JWT
            //return httpContext.User.Identity?.IsAuthenticated == true &&
            //        httpContext.User.IsInRole("Admin");
        }
    }
}

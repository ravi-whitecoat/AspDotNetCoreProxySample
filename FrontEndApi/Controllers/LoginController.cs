using System.Collections.Generic;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Mvc;

namespace FrontEndApi.Controllers
{
    [Produces("application/json")]
    public class LoginController : Controller
    {
        [Route("api/auth/sign-in")]
        public async Task<string> SignIn()
        {
            var claims = new List<Claim>
            {
                new Claim(ClaimTypes.Name, "test@test.com")
            };

            var userIdentity = new ClaimsIdentity(claims, "login");
            var principal = new ClaimsPrincipal(userIdentity);

            await HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, principal);
            return HttpContext.User.Identity.IsAuthenticated.ToString();
        }

        [Route("api/auth/status")]
        public string Status()
        {
            return HttpContext.User.Identity.IsAuthenticated.ToString();
        }

        [Route("api/auth/sign-out")]
        public async Task<string> SignOut()
        {
            await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
            return HttpContext.User.Identity.IsAuthenticated.ToString();
        }
    }
}
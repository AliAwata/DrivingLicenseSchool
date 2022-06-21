using Microsoft.AspNet.Identity;
using Microsoft.Owin;
using Microsoft.Owin.Security.Cookies;
using Owin;

namespace DrivingSclApp
{
    public partial class Startup
    {
        public void ConfigureAuth(IAppBuilder app)
        {
            app.UseCookieAuthentication(new CookieAuthenticationOptions
            {
                AuthenticationType = DefaultAuthenticationTypes.ApplicationCookie,
                LoginPath = new PathString("/Account/Login"),
                CookieName = string.Format("DrivingScl"),
                ReturnUrlParameter = "returnUrl",
                ExpireTimeSpan = new System.TimeSpan(0, 60, 0),
                SlidingExpiration = true,
                CookiePath = "/",
            });
        }
    }
}
using System.Web.Mvc;

namespace DrivingSclApp.Areas.HandleExceptions
{
    public class HandleExceptionsAreaRegistration : AreaRegistration 
    {
        public override string AreaName 
        {
            get 
            {
                return "HandleExceptions";
            }
        }

        public override void RegisterArea(AreaRegistrationContext context) 
        {
            context.MapRoute(
                "HandleExceptions_default",
                "HandleExceptions/{controller}/{action}/{id}",
                new { action = "Index", id = UrlParameter.Optional }
            );
        }
    }
}
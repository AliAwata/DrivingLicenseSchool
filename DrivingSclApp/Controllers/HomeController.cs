using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace DrivingSclApp.Controllers
{
    [Authorize, Audit, CanDoIt, NoCache, RedirectOnError]
    public class HomeController : Controller
    {
        public ActionResult Index()
        {
            ViewData["PageName"] = "الصفحة الرئيسية";
            ViewBag.logUser = Session["UserName"];
            return View();
        }

        public ActionResult About()
        {
            ViewData["PageName"] = "حول التطبيق";
            return View();
        }
        public ActionResult ContactUs()
        {
            ViewData["PageName"] = "اتصل بنا";
            return View();
        }

        public ActionResult ViewImage(string FileName)
        {
            ViewData["PageName"] = "استعلام";
            //string s  = "d:\\Images\\" + FileName;
            ViewData["FileName"] = "/DocImages/" + FileName;
            return PartialView("ImageContent");
        }
    }
}
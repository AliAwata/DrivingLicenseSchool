using DrivingSclApp.Models;
using Kendo.Mvc.UI;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.Owin;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace DrivingSclApp.Controllers
{
    [Authorize, NoCache, RedirectOnError]
    public class AccountController : Controller
    {
        private AppsManagerEntities db = new AppsManagerEntities();
        public ActionResult Index()
        {
            var user = new DAOUsers();
            user.Username = Session["UserName"].ToString();
            user = DAOUsers.SelectAllBySearchFields(user).SingleOrDefault(u => u.Username.ToLower() == user.Username.ToLower() && u.Locked != "0");
            double d = MyDataBase.GetRowsPerPage(user.Nb);
            double dd = MyDataBase.GetReportType(user.Nb);
            ViewData["UserName"] = user.Username;
            ViewData["RowsPerPage"] = d;
            ViewData["PrintOption"] = MyDataBase.GetPrintOptionList(dd.ToString());
            ViewBag.Alert = "";
            return View();
        }
        [HttpPost]
        public ActionResult Index(string PrintOption, string RowsPerPage, FormCollection FormCollection)
        {
            string pNb = MyDataBase.RunQuery("SELECT NB FROM APPMGR.PROGRAMS WHERE NAME='" + Resource.ProgID + "'");
            var user = new DAOUsers();
            user.Username = Session["UserName"].ToString();
            user = DAOUsers.SelectAllBySearchFields(user).SingleOrDefault(u => u.Username.ToLower() == user.Username.ToLower() && u.Locked != "0");
            string sql = "BEGIN\n";
            sql += "  INSERT INTO AppMgr.SETTINGS VALUES(null,'REPORT_TYPE'," + PrintOption + ",1,'',null,null,null," + user.Nb + "," + pNb + ");\n";
            sql += "EXCEPTION WHEN OTHERS THEN \n";
            sql += "UPDATE AppMgr.SETTINGS SET VAL=" + PrintOption;
            sql += " WHERE NAME='REPORT_TYPE' AND USERNB=" + user.Nb + " and ProgNb=" + pNb + ";\n";
            sql += " END;";
            MyDataBase.ExecQuery(sql);
            sql = "BEGIN\n";
            sql += "  INSERT INTO AppMgr.SETTINGS VALUES(null,'ROWS_PER_PAGE'," + RowsPerPage + ",1,'',null,null,null," + user.Nb + "," + pNb + ");\n";
            sql += "EXCEPTION WHEN OTHERS THEN \n";
            sql += "UPDATE AppMgr.SETTINGS SET VAL=" + RowsPerPage;
            sql += " WHERE NAME='ROWS_PER_PAGE' AND USERNB=" + user.Nb + " and ProgNb=" + pNb + ";\n";
            sql += " END;";
            MyDataBase.ExecQuery(sql);
            ViewData["RowsPerPage"] = MyDataBase.GetRowsPerPage(user.Nb);
            ViewData["PrintOption"] = MyDataBase.GetPrintOptionList(MyDataBase.GetReportType(user.Nb).ToString());
            ViewBag.Alert = "Updated";
            ViewData["Msg"] = "تم تسجيل الإعدادات بنجاح";
            return View();
        }
        [AllowAnonymous]
        public ActionResult Login(string returnUrl)
        {
            ViewBag.ReturnUrl = returnUrl;
            ViewData["PageName"] = "تسجيل الدخول";
            return View();
        }

        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public ActionResult Login(LoginViewModel model, string returnUrl)
        {
            if (ModelState.IsValid)
            {
                Audit.AuditRecord audit = new Audit.AuditRecord();
                audit.ControllerName = "DrivingSclApp.Controllers.AccountController";
                audit.Area = "";
                audit.ActionName = "Login";
                audit.IP = Request.UserHostAddress;
                audit.User_Name = model.UserName;
                audit.Application = Resource.ProgName;
                audit.AuditedFields_New = "";

                ViewData["theDB"] = MyDataBase.GetDBList("Oracle");


                UserAccountManager uam = new UserAccountManager(db);
                if (uam.Login(model) == SignInStatus.Success)
                {
                    MyOwnData.SetCookie("UserName", model.UserName);
                    DAOUsers loggedUser = new DAOUsers();
                    loggedUser.Username = model.UserName.ToLower();
                    loggedUser = DAOUsers.SelectAllBySearchFields(loggedUser).Where(u => u.Username.ToLower() == model.UserName.ToLower()).FirstOrDefault();
                    Session["UserNb"] = loggedUser.Nb;
                    Session["CtyNb"] = loggedUser.Ctynb;
                    if (loggedUser.Ctynb.HasValue)
                        MyOwnData.SetCookie("CtyNb", loggedUser.Ctynb.ToString());
                    else
                        MyOwnData.SetCookie("CtyNb", "0");
                    Session["UserName"] = model.UserName;
                    audit.User_NB = loggedUser.Nb;

                    if (loggedUser.Nb != 0)
                    {
                        string sql = "SELECT 1 FROM APPMGR.V_USERPROG WHERE ProgId ='" + Resource.ProgID + "'\n";
                        sql += "AND USERNB=" + loggedUser.Nb;
                        if (string.IsNullOrEmpty(MyDataBase.RunQuery(sql)))
                            ModelState.AddModelError("", model.UserName + ": لا يحق لك الدخول إلى النظام، يرجى مراجعة مدير النظام.");
                    }
                    string qr = "SELECT 1 FROM APPMGR.USERS WHERE NB=" + loggedUser.Nb + " AND ((LOCKED is null) or (LOCKED <> '0')) ";
                    if (string.IsNullOrEmpty(MyDataBase.RunQuery(qr)))
                        ModelState.AddModelError("", "لا يمكنك الدخول لأن حسابك مقفل. راجع مدير النظام");
                    MyOwnData.SetCookie("MyNB", loggedUser.Nb.ToString());
                    string uname = loggedUser.Username;
                    Audit.AddLog(audit, "Login", "", "");
                    if (!string.IsNullOrEmpty(returnUrl) && Url.IsLocalUrl(returnUrl))
                    {
                        return Redirect(returnUrl);
                    }
                    else
                    {
                        return RedirectToAction("Index", "Home");
                    }
                }
                else
                {
                    ModelState.AddModelError("", "اسم المستثمر أو كلمة المرور غير صحيحة.");
                }
                Audit.AddLog(audit, "Login", "", "");
            }
            return View(model);
        }

        public ActionResult LogOff()
        {
            HttpContext.GetOwinContext().Authentication.SignOut();
            return RedirectToAction("Login", "Account");
        }

        public ActionResult ChangePassword()
        {
            int t = 20;
            List<SelectListItem> list1 = new List<SelectListItem>();
            SelectListItem sle1 = new SelectListItem
            {
                Text = "5",
                Value = "5",
                Selected = (t == 5)
            };
            list1.Add(sle1);

            SelectListItem sle2 = new SelectListItem
            {
                Text = "10",
                Value = "10",
                Selected = (t == 10)
            };
            list1.Add(sle1);

            SelectListItem sle3 = new SelectListItem
            {
                Text = "20",
                Value = "20",
                Selected = (t == 20)
            };
            list1.Add(sle3);
            ViewBag.DefaultRecordsPerPage = list1;

            List<SelectListItem> list2 = new List<SelectListItem>();
            int tt = 2;
            SelectListItem s1 = new SelectListItem
            {
                Text = "Excel",
                Value = "1",
                Selected = (tt == 1)
            };
            list2.Add(s1);
            SelectListItem s2 = new SelectListItem
            {
                Text = "Word",
                Value = "2",
                Selected = (tt == 2)
            };
            list2.Add(s2);
            SelectListItem s3 = new SelectListItem
            {
                Text = "PDF",
                Value = "3",
                Selected = (tt == 3)
            };
            list2.Add(s3);

            SelectListItem s4 = new SelectListItem
            {
                Text = "HTML",
                Value = "4",
                Selected = (tt == 4)
            };
            list2.Add(s4);
            ViewBag.DefaultPrintOption = list2;
            ChangePasswordViewModel model = new ChangePasswordViewModel
            {
                DefaultPrintOption = tt,
                DefaultRecordsPerPage = t,
            };
            return View(model);
        }

        [HttpPost]
        [ActionName("ChangePassword")]
        public ActionResult ChangePassword(ChangePasswordViewModel model)
        {
            var ph = new PasswordHasher();
            //string pwd = ph.HashPassword(loginModel.Password);
            string uName = Session["UserName"].ToString();
            var user = db.USERS.SingleOrDefault(u => u.Username == uName);
            var res = ph.VerifyHashedPassword(user.Password, model.OldPassword);
            if (user == null || res != PasswordVerificationResult.Success)
            {
                return this.Json(new DataSourceResult
                {
                    Errors = "تأكد من كلمة المرور القديمة"
                });
            }
            if (model.NewPassword != model.NewPasswordConfirmed)
            {
                return this.Json(new DataSourceResult
                {
                    Errors = "تأكد من تطابق كلمة المرور الجديدة مع التأكيد"
                });
            }
            else
            {
                var query = (from q in db.USERS
                             where q.Nb == user.Nb
                             select q).First();
                query.Password = ph.HashPassword(model.NewPassword);
                db.SaveChanges();
                // return result;
                return LogOff();
            }
        }
        protected override void Dispose(bool disposing)
        {
            db.Dispose();
            base.Dispose(disposing);
        }
        public ActionResult RegisterApplication()
        {
            ViewData["PageName"] = "تسجيل التطبيق";
            string s;
            string Registred = MyDataBase.RunQuery_reader("select 1 from AppMgr.PROGRAMS where Name = '" + Resource.ProgID + "'");
            if (Registred == "0")
            {
                double x = MyDataBase.ExecQuery("INSERT INTO AppMgr.PROGRAMS(NAME,NOTE) VALUES ('" + Resource.ProgID + "', '" + Resource.ProgName + "')");
                MvcHelper.AddControllerActions();
                MvcHelper.initPerms();
                s = x == 1 ? "تم تسجيل النظام بنجاح" : "لم يتم تسجيل النظام بنجاح. يرجى التحقق.";
                ViewBag.msg = s;
            }
            else { s = "عفواً، البرنامج مسجل مسبقاً!"; }

            return RedirectToAction("Index", "Home", new { msgData = s });
        }
    }
}

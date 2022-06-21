using Microsoft.AspNet.Identity;
using Oracle.ManagedDataAccess.Client;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Web;
using System.Web.Mvc;
using System.Web.Routing;
using System.Xml;

namespace DrivingSclApp
{
    public static class MyResource
    {
        public static void setpassword(string nb, string pid)
        {
            var ph = new PasswordHasher();
            string sql2 = "Update AppMgr.Users set PASSWORD = '" + ph.HashPassword(pid) + "' WHERE NB=" + nb;
            MyDataBase.ExecQuery(sql2);
        }
        public static string GetHeading(int i)
        {
            try
            {
                return MyDataBase.RunQuery("select name from Con_3roudIn.headers where nb=" + i.ToString());
            }
            catch { return Resource.ResourceManager.GetString("Heading" + i.ToString()); }
        }
    }
    public static class MvcHelper
    {
        public static void initPerms()
        {
            string sql = "INSERT INTO APPMGR.ROLES (NAME) VALUES(" + "'DefaultGoveComplain') ";
            double roleNb = MyDataBase.ExecQueryRtNB(sql);

            string appmgrActions = "SELECT A.* FROM APPMGR.ACTIONS a INNER JOIN APPMGR.CONTROLLERS C ON C.NB = A.CONTROLLERNB "
                + " INNER JOIN APPMGR.PROGRAMS P ON P.NB = C.PROGNB where P.NAME = '" + Resource.ProgName.ToString() + "' ";
            // sql = "select  * from  where NAME = 'DefaultAppMGR'";
            string retValue = "";
            //            object ret;
            OracleConnection conn = new OracleConnection(MyDataBase.GetConnectionString());
            List<int> acts = new List<int>();

            try
            {
                OracleCommand cmd = new OracleCommand(appmgrActions, conn);
                conn.Open();
                OracleDataReader dr = cmd.ExecuteReader();
                while (dr.Read())
                {
                    retValue = dr.GetValue(0).ToString();
                    acts.Add(int.Parse(retValue));
                }
            }
            catch (Exception e)
            {
                retValue = "0";
            }
            finally
            {
                conn.Close();
                conn.Dispose();
            }

            string deefaultRoleNb = "select * from  APPMGR.ROLES where NAME ='DefaultGoveComplain' ";
            conn = new OracleConnection(MyDataBase.GetConnectionString());

            try
            {
                OracleCommand cmd = new OracleCommand(deefaultRoleNb, conn);
                conn.Open();
                OracleDataReader dr = cmd.ExecuteReader();
                while (dr.Read())
                {
                    roleNb = int.Parse(dr.GetValue(0).ToString());
                }
            }
            catch (Exception e)
            {
                roleNb = 0;
            }
            finally
            {
                conn.Close();
                conn.Dispose();
            }
            foreach (var i in acts)
            {
                sql = "INSERT INTO APPMGR.ROLE_ACTIONS (ROLENB, ACTIONNB) VALUES( " + roleNb + "," + (int)i + ") ";
                MyDataBase.ExecQuery(sql);
            }

            string adminUser = "select * from APPMGR.users where USERNAME = 'Admin' ";
            int userId = 0;
            try
            {
                OracleCommand cmd = new OracleCommand(adminUser, conn);
                conn.Open();
                OracleDataReader dr = cmd.ExecuteReader();
                while (dr.Read())
                {
                    userId = int.Parse(dr.GetValue(0).ToString());
                }
            }
            catch (Exception e)
            {
                retValue = "0";
            }
            finally
            {
                conn.Close();
                conn.Dispose();
            }
            sql = "INSERT INTO APPMGR.USER_ROLES ( ROLENB, USERNB) VALUES(" + roleNb + "," + userId + ") ";
            MyDataBase.ExecQuery(sql);
        }
        public static List<string> GetActions(this Type controller)
        {
            return controller.GetMethods()
                .Where(x => (!x.Name.Equals("ToString") && (x.ReturnType == typeof(ActionResult) || x.ReturnType == typeof(string)) && (!Attribute.IsDefined(x, typeof(SkipCanDoItFilterAttribute)) )))
                .OrderBy(x => x.Name)
                .Select(x => x.Name)
                .Distinct()
                .ToList();
        }

        private static List<Type> GetSubClasses<T>()
        {
            return Assembly.GetCallingAssembly().GetTypes()
                .Where(type => type.IsSubclassOf(typeof(T)))
                .ToList();
        }

        public static List<string> GetControllerNames()
        {
            List<string> controllerNames = new List<string>();
            GetSubClasses<Controller>()
                .ForEach(type => controllerNames.Add(type.Name));
            return controllerNames;
        }

        public static void AddControllerActions()
        {
            #region Standard Controllers' Names
            Dictionary<string, string> dictControllers = new Dictionary<string, string>();
            foreach (Type controller in GetSubClasses<Controller>())
            {
                dictControllers.Add(controller.ToString(), controller.Name);
            }

            dictControllers["GoveComplain.Controllers.AccountController"] = "صفحة البدء";
            dictControllers["GoveComplain.Controllers.HomeController"] = "الصفحة الرئيسية";

            dictControllers["GoveComplain.Areas.Complain.Controllers.COMPLAINSController"] = "صفحة الشكاوي";
            dictControllers["GoveComplain.Areas.Indexes.Controllers.COMPLAIN_CATEGORYController"] = "تصنيف الشكاوي";
            dictControllers["GoveComplain.Areas.Indexes.Controllers.SERVICE_PROVIDERController"] = "مقدم الخدمة";
            dictControllers["GoveComplain.Areas.Indexes.Controllers.ZCITYController"] = "المدن";
            dictControllers["GoveComplain.Areas.Indexes.Controllers.ZDEPARTMENTController"] = "الاقسام";
            dictControllers["GoveComplain.Areas.Indexes.Controllers.ZMTYPEController"] = "نوع المحل المخالف";
            dictControllers["GoveComplain.Areas.Indexes.Controllers.ZREGONController"] = "المناطق";
            dictControllers["GoveComplain.Areas.Indexes.Controllers.ZSTATEController"] = "الحالة";
            dictControllers["GoveComplain.Areas.Indexes.Controllers.ZTYPESController"] = "نوع المادة المخالفة";
            dictControllers["GoveComplain.Areas.Indexes.Controllers.ZAuditorController"] = "صفحة المراقبين";
            dictControllers["GoveComplain.Areas.Complain.Controllers.COMMITTEESController"] = "صفحة الضبوط والمراقبين";
            dictControllers["GoveComplain.Areas.Indexes.Controllers.GenPartController"] = "الجهات العامة";
            dictControllers["GoveComplain.Areas.HandleExceptions.Controllers.HandleExceptionController"] = "معالجة الأخطاء";
            #endregion

            Dictionary<string, string> dictActions = new Dictionary<string, string>();
            #region Common Actions
            //CRUD Actions
            dictActions.Add("Create", "إضافة سجل جديد");
            dictActions.Add("Edit", "تعديل السجل");
            dictActions.Add("Delete", "حذف السجل");
            dictActions.Add("Destroy", "حذف السجل");
            dictActions.Add("Sort", "ترتيب");
            dictActions.Add("Details", "عرض التفاصيل");
            dictActions.Add("First", "الانتقال إلى الصفحة الأولى");
            dictActions.Add("Prev", "الانتقال إلى الصفحة السابقة");
            dictActions.Add("Next", "الانتقال إلى الصفحة التالية");
            dictActions.Add("Last", "الانتقال إلى الصفحة الأخيرة");
            dictActions.Add("Print", "طباعة التقرير");
            dictActions.Add("Search", "بحث");
            dictActions.Add("ViewResult", "عرض النتائج");
            dictActions.Add("Show", "عرض");
            dictActions.Add("EditingPopup_Read", "استعلام");
            dictActions.Add("Read", "استعلام");
            dictActions.Add("EditingPopup_Update", "تعديل");
            dictActions.Add("Update", "تعديل");
            dictActions.Add("EditingPopup_Destroy", "حذف");
            dictActions.Add("Search_Read", "بحث");
            // Account Actions
            dictActions.Add("Index", "عرض واستعلام");
            dictActions.Add("Login", "تسجيل الدخول");
            dictActions.Add("LogOff", "تسجيل الخروج");
            dictActions.Add("ChangePassword", "تغيير كلمة المرور");
            dictActions.Add("ChangePasswordSuccess", "عرض صفحة نجاح تغيير كلمة المرور");
            // Dumuzi Actions
            dictActions.Add("Find", "بحث عن عناصر مشابهة");
            //Home Actions
            dictActions.Add("About", "حول التطبيق");
            dictActions.Add("Contact", "اتصل بنا");
            //Admin Home actions
            dictActions.Add("RegisterApplication", "تسجيل التطبيق");
            dictActions.Add("AuditApplication", "تسجيل التطبيق للمراقبة");
            dictActions.Add("GrantApplication", "تسجيل التطبيق للصلاحيات");
            // Handel Exception Actions
            dictActions.Add("AddLog", "تسجيل الأحداث");
            dictActions.Add("MailTo", "إرسال بريد إلى مدير النظام");
            #endregion
            #region SpecialActions
            // Complain Actions
            dictActions.Add("IndexAudit", "استعلام المراقبين");
            dictActions.Add("CreateComm", "إضافة لجنة");
            dictActions.Add("GetPartialView_cardowner", "بحث اسماء المراقبين");
            dictActions.Add("GetAuditors", "عرض اسماء المراقبين");
            dictActions.Add("GetZMTYPE", "عرض نوع المادة المخالفة");
            dictActions.Add("GetMTYPE", "عرض نوع المادة المخالفة");
            dictActions.Add("DeleteAuditors", "حذف مراقب الشكوى");
            dictActions.Add("IndexADJUST", "عرض ضبط");
            dictActions.Add("Aggregates", "تجميع شكوى");
            dictActions.Add("Aggregates_Read", "تجميع شكوى");
            dictActions.Add("Editing_Popup", "تعديل");
            dictActions.Add("_EditComplain", "تعديل شكوى");
            dictActions.Add("GetDistrict", "تجميع");
            dictActions.Add("CreateADJUST", "انشاء ضبط");
            dictActions.Add("EditingPopup_ReadADJUST", "تعديل ضبط");
            dictActions.Add("EditingPopup_Create", "تعديل2");
            dictActions.Add("EditingPopup_UpdateADJUST", "تحديث ضبط");
            dictActions.Add("EditingPopup_DestroyADJUST", "حذف ضبط");
            dictActions.Add("Search_Read_By_NB", "بحث بالرقم المميز");
            dictActions.Add("Result", "النتيجة");
            dictActions.Add("Result_Update", "نحديث النتيجة");
            dictActions.Add("HideShow", "اخفاء/اظهار");
            dictActions.Add("HideSource", "مصدر الشكوى");
            dictActions.Add("websearch", "بحث موبايل");
            dictActions.Add("RetrieveImage", "عرض المرفق");
            dictActions.Add("GetZTYPES", "عرض نوع المحل المخالف");
            dictActions.Add("ZAudit_Read", "صفحة فهرس أسماء المراقبين");
            dictActions.Add("GetPartialView_COMPLAINS", "اختيار شكوى للضبط");
            dictActions.Add("AddReport", "إضافة ضبط لشكوى");
            dictActions.Add("AddResult", "تسجيل نتيجة الشكوى");
            dictActions.Add("ForwardComplain", "إحالة الشكوى");
            // COMMITTEES Actions
            dictActions.Add("EditingPopup_Read2", "مراقبي الضبوط");
            dictActions.Add("DeleteConfirm1", "حذف مراقب من ضبط");
            // COMPLAINSAPI actions     
            #endregion
            string pNb;
            pNb = MyDataBase.RunQuery("SELECT NB FROM APPMGR.PROGRAMS WHERE NAME='" + Resource.ProgID + "'");
            foreach (Type controller in GetSubClasses<Controller>())
            {
                string sql = "INSERT INTO APPMGR.CONTROLLERS (PROGNB, FULLNAME, ENAME, ANAME)  VALUES(" + pNb + ", " +//PROGNB
                    "'" + controller.ToString() + "', " +//FULLNAME
                    "'" + controller.Name + "', " +//ENAME
                    "'" + dictControllers[controller.ToString()] + "') RETURNING NB INTO :insertedNB";//ANAME

                double cnb = MyDataBase.ExecQueryRtNB(sql);
                foreach (string action in controller.GetActions())
                {
                    try
                    {
                        sql = "INSERT INTO APPMGR.ACTIONS (CONTROLLERNB, FULLNAME, ENAME, ANAME,   ISPUBLIC, HASFILE, IS_AUDIT, IS_GRANT) VALUES(" +
                            "'" + cnb + "', " +
                            "'" + action + "', " +
                            "'" + action + "', " +
                        //    "'" + controller.Name + "', " +
                        "'" + dictActions[action] + "', 0, 0, 1, 1) ";
                        MyDataBase.ExecQuery(sql);
                    }
                    catch
                    {
                        sql = "INSERT INTO APPMGR.ACTIONS (CONTROLLERNB, FULLNAME, ENAME, ANAME,   ISPUBLIC, HASFILE, IS_AUDIT, IS_GRANT) VALUES(" +
                            "'" + cnb + "', " +
                            "'" + action + "', " +
                            "'" + action + "', " +
                        //    "'" + controller.Name + "', " +
                        "'" + action + "', 0, 0, 1, 1) ";
                        MyDataBase.ExecQuery(sql);
                    }
                }
            }
        }
    }
    public class SkipAuditFilterAttribute : Attribute
    {
    }
    public class Audit : ActionFilterAttribute
    {
        public static XmlDocument GetXmlTable(string TableName, string KeyValue)
        {
            XmlDocument xml = new XmlDocument();
            OracleConnection conn = new OracleConnection(MyDataBase.GetConnectionString());
            try
            {
                conn.Open();
                OracleCommand cmd = new OracleCommand("SELECT *  FROM " + TableName + " where NB =" + KeyValue, conn);
                OracleDataReader dr = cmd.ExecuteReader();
                DataTable MyTable = new DataTable(TableName);

                MyTable.Load(dr);
                conn.Close();
                conn.Dispose();
                System.IO.StringWriter writer = new System.IO.StringWriter();
                MyTable.WriteXml(writer, true);
                xml.LoadXml(writer.ToString());
                string Id = xml.FirstChild.InnerXml;
                xml.LoadXml(Id);
            }
            catch
            { xml = null; }
            finally
            {
                conn.Close();
                conn.Dispose();
            }
            return xml;
        }
        public override void OnActionExecuting(ActionExecutingContext filterContext)
        {
            if (filterContext.ActionDescriptor.GetCustomAttributes(typeof(SkipAuditFilterAttribute), false).Any())
            {
                return;
            }
            bool audited = MyDataBase.RunQuery("SELECT AUDITED FROM appmgr.PROGRAMS WHERE Name='" + Resource.ProgID + "'") == "1";
            audited &= (
                        (filterContext.RequestContext.HttpContext.Request.HttpMethod.ToUpper() == "POST")
                        ||
                        (filterContext.RouteData.Values["action"].ToString().Contains("Delete"))
                        ||
                        (filterContext.RouteData.Values["action"].ToString().Contains("DocCard"))
                        ||
                        (filterContext.RouteData.Values["action"].ToString().Contains("PrintInformDoc"))
                        ||
                        (filterContext.RouteData.Values["action"].ToString().Contains("MakeCopy"))
                        ||
                        (filterContext.RouteData.Values["action"].ToString().Contains("PrintCopy"))
                        ||
                        (filterContext.RouteData.Values["action"].ToString().Contains("AlterDoc"))
                        ||
                        (filterContext.RouteData.Values["action"].ToString().Contains("Waiver"))
                        ||
                        (filterContext.RouteData.Values["action"].ToString().Contains("ValidateWaiver"))
                        ||
                        (filterContext.RouteData.Values["action"].ToString().Contains("AlarmDoc"))
                        ||
                        (filterContext.RouteData.Values["action"].ToString().Contains("ValidateAlarm"))
                        ||
                        (filterContext.RouteData.Values["action"].ToString().Contains("DocWithParent"))
                        ||
                        (filterContext.RouteData.Values["action"].ToString().Contains("ValidateNewDoc"))
                        ||
                        (filterContext.RouteData.Values["action"].ToString().Contains("FixInformDoc"))
                        ||
                        (filterContext.RouteData.Values["action"].ToString().Contains("CancelCont"))
                        ||
                        (filterContext.RouteData.Values["action"].ToString().Contains("ValidateCancelCont"))
                       );
            if (audited)
            {
                string param = "";
                AuditRecord audit = new AuditRecord();
                audit.ControllerName = filterContext.Controller.GetType().ToString();
                audit.Area = filterContext.RouteData.DataTokens.ContainsKey("area") ? filterContext.RouteData.DataTokens["area"].ToString() : "";
                audit.ActionName = filterContext.RouteData.Values["action"].ToString();
                audit.IP = filterContext.RequestContext.HttpContext.Request.UserHostAddress;
                audit.User_NB = long.Parse(HttpContext.Current.Session["UserNB"].ToString());
                audit.User_Name = HttpContext.Current.Session["UserName"].ToString();
                audit.Application = Resource.ProgID;
                audit.AuditedFields_New = param;
                string Table_name = "";
                string Id = "";
                foreach (KeyValuePair<string, object> parameter in filterContext.ActionParameters)
                {
                    if (parameter.Value != null)
                    {
                        if ((parameter.Key != "request") &&
                            (audit.ActionName.Contains("Create")
                            || audit.ActionName.Contains("Upda")
                            || audit.ActionName.Contains("Destroy")
                            || audit.ActionName.Contains("Edit")
                            || audit.ActionName.Contains("Delete")
                            || audit.ActionName.Contains("NewSession")
                            || audit.ActionName.Contains("DocValide")
                            ))
                        {
                            if (parameter.Key != "fileToAdd")
                                try
                                {
                                    XmlDocument xml = new XmlDocument();
                                    xml.LoadXml(parameter.Value.ToString());
                                    Id = xml.FirstChild.FirstChild.InnerText;
                                    Table_name = xml.DocumentElement.Name;
                                    audit.AuditedFields_New = xml.InnerXml;
                                }
                                catch
                                {
                                    audit.AuditedFields_New = parameter.Value.ToString();
                                }
                        }
                        else
                        {
                            if (parameter.Key != "request")
                            {
                                audit.AuditedFields_New = audit.AuditedFields_New + " " + parameter.Key + " :  " + parameter.Value;
                            }
                        }
                    }
                }
                if (audit.ActionName.Contains("Create"))
                {
                    AddLog(audit, "Create", "", "");
                }
                else if (audit.ActionName.Contains("Create")
                            || audit.ActionName.Contains("Upda")
                            || audit.ActionName.Contains("Destroy")
                            || audit.ActionName.Contains("Edit")
                            || audit.ActionName.Contains("Delete")
                            || audit.ActionName.Contains("NewSession")
                            || audit.ActionName.Contains("DocValide")
                            )
                {
                    XmlDocument xmlNew = new XmlDocument();
                    try
                    {
                        xmlNew = GetXmlTable(Table_name, Id);
                        audit.AuditedFields_Old = xmlNew.InnerXml;
                    }
                    catch
                    {
                        audit.AuditedFields_Old = "";
                    }

                    AddLog(audit, audit.ActionName, "", "");
                }
                else
                {
                    AddLog(audit, "", "", "");
                }
            }
            base.OnActionExecuting(filterContext);
        }
        public class AuditRecord
        {
            public string AuditedFields_New { get; set; }
            public string AuditedFields_Old { get; set; }
            public string EntityTypeAudited { get; set; }
            public DateTime DateTimeAuditRecorded { get; set; }
            public string User_Name { get; set; }
            public long? User_NB { get; set; }
            public string ControllerName { get; set; }
            public string ActionName { get; set; }
            public string Area { get; set; }
            public string Application { get; set; }
            public string IP { get; set; }
        }
        public static void AddLog(AuditRecord audit, string type, string KeyName, string keyValue)
        {
            string sql = "INSERT INTO appmgr.LOG_ACTION (nb , ldate ,USER_NB, USER_NAME, CONTROLLER, ACTION, PARAMS, PARAMS_old , APPLICATION, CLIENTIP)";
            sql += " VALUES (appmgr.MAIN_SEQUENCE.nextval , sysdate  , ";
            sql += "'" + audit.User_NB + "', '" + audit.User_Name + "', '" + audit.ControllerName + "', '" + audit.ActionName + "' ,'" + audit.AuditedFields_New + "' , '" + audit.AuditedFields_Old + "' , '" + audit.Application + "' , '" + audit.IP + "')";
            OracleConnection conn = new OracleConnection(MyDataBase.GetConnectionString());
            try
            {
                conn.Open();
                OracleCommand cmd = new OracleCommand(sql, conn);

                cmd.ExecuteNonQuery();
            }
            catch
            {
            }
            finally
            {
                conn.Close();
                conn.Dispose();
            }
        }
    }
    public class SkipCanDoItFilterAttribute : Attribute
    {
    }
    [Authorize]
    public class CanDoIt : ActionFilterAttribute
    {
        public bool UserCanDoAction(string usernb, string controller, string action)
        {
            if (string.IsNullOrEmpty(usernb)) { return false; }
            if (HttpContext.Current.Session["UserName"].ToString() == "Admin")
                return true;
            usernb = HttpContext.Current.Session["UserNB"].ToString();
            string val;
            if (string.IsNullOrEmpty(usernb))
                return false;
            if (usernb.Equals("0"))
                return true;

            string canQuery = " SELECT A.* FROM APPMGR.ACTIONS a INNER JOIN APPMGR.CONTROLLERS C ON C.NB = A.CONTROLLERNB "
                + " INNER JOIN APPMGR.PROGRAMS P ON P.NB = C.PROGNB  "
                + " WHERE   C.FULLNAME = '" + controller + "' AND A.FULLNAME = '" + action + "'"
                + " AND P.NAME = '" + Resource.ProgID.ToString() + "'";
            val = MyDataBase.RunQuery_reader(canQuery);
            if (val.Equals("0"))
                return true;

            canQuery = " SELECT A.* FROM APPMGR.ACTIONS a INNER JOIN APPMGR.ROLE_ACTIONS ra ON A.NB = RA.ACTIONNB INNER JOIN APPMGR.CONTROLLERS C ON C.NB = A.CONTROLLERNB "
              + " INNER JOIN APPMGR.PROGRAMS P ON P.NB = C.PROGNB INNER JOIN APPMGR.ROLES RL ON RL.NB = RA.ROLENB INNER JOIN APPMGR.USER_ROLES UR ON UR.ROLENB = RL.NB"
              + " WHERE UR.USERNB = " + usernb + " AND C.FULLNAME = '" + controller + "' AND A.FULLNAME = '" + action + "'"
              + " AND P.NAME = '" + Resource.ProgID.ToString() + "'";

            val = MyDataBase.RunQuery_reader(canQuery);
            if (val.Equals("0"))
                return false;

            return true;
        }
        public override void OnActionExecuting(ActionExecutingContext filterContext)
        {
            if (filterContext.ActionDescriptor.GetCustomAttributes(typeof(SkipCanDoItFilterAttribute), false).Any())
            {
                return;
            }

            string controller = filterContext.Controller.GetType().ToString();
            string action = filterContext.RouteData.Values["action"].ToString();
            filterContext.Controller.TempData["Controller"] = controller;
            filterContext.Controller.TempData["Action"] = action;
            string usernb = "";
            try
            {
                usernb = filterContext.HttpContext.Session["UserNb"].ToString();
                if (!UserCanDoAction(usernb, controller, action))
                {
                    string q = "";
                    int? controllernb = 0;
                    filterContext.Controller.TempData["exception_type"] = "خطأ صلاحيات";
                    try
                    {
                        q = "SELECT ANAME FROM appmgr.CONTROLLERS WHERE FULLNAME='" + controller + "'";
                        filterContext.Controller.TempData["Controller"] = MyDataBase.RunQuery(q);
                        q = "SELECT NB FROM appmgr.CONTROLLERS WHERE FULLNAME= '" + controller + "'";
                        controllernb = int.Parse(MyDataBase.RunQuery(q));
                        q = "SELECT ANAME FROM appmgr.ACTIONS WHERE CONTROLLERNB=" + controllernb + " AND FULLNAME='" + action + "'";
                        filterContext.Controller.TempData["Action"] = MyDataBase.RunQuery(q);
                    }
                    catch
                    {

                        filterContext.Controller.TempData["Error"] = "لا تملك الصلاحية اللازمة للعملية التي اخترتها";
                        filterContext.Controller.TempData["StackTrace"] = "";

                    }

                    filterContext.Controller.TempData["Error"] = "لا تملك الصلاحية اللازمة للعملية التي اخترتها";
                    filterContext.Controller.TempData["StackTrace"] = "";

                    // filterContext.RedirectToAction("DataEntry/HandleException/Index");
                    //   filterContext.Result = new RedirectResult("DataEntry/HandleException/Index");
                    //if (filterContext.HttpContext.Request.HttpMethod == "POST")
                    //{
                    //    filterContext.Result = new RedirectToRouteResult(new route)
                    //}
                    //else
                    filterContext.Result = new RedirectToRouteResult(new RouteValueDictionary(new { controller = "HandleException", action = "Index", area = "HandleExceptions" }));


                }
            }
            catch
            {
                filterContext.Result = new RedirectToRouteResult(new RouteValueDictionary(new { controller = "Account", action = "LogOff", area = "" }));
            }
            base.OnActionExecuting(filterContext);
        }
    }
    public class NoCache : ActionFilterAttribute
    {

        public override void OnResultExecuting(ResultExecutingContext filterContext)
        {
            string controller = filterContext.Controller.GetType().Name;
            string action = filterContext.RouteData.Values["action"].ToString();
            filterContext.Controller.ViewData["Controller"] = controller.Replace("Controller", "");
            filterContext.Controller.ViewData["Action"] = action;
            HttpContext.Current.Response.Cache.SetExpires(DateTime.UtcNow.AddDays(-1));
            HttpContext.Current.Response.Cache.SetValidUntilExpires(false);
            HttpContext.Current.Response.Cache.SetRevalidation(HttpCacheRevalidation.AllCaches);
            HttpContext.Current.Response.Cache.SetCacheability(HttpCacheability.NoCache);
            HttpContext.Current.Response.Cache.SetNoStore();
            base.OnResultExecuting(filterContext);
        }
        public override void OnResultExecuted(ResultExecutedContext filterContext)
        {
            base.OnResultExecuted(filterContext);
        }
    }

    public class RedirectOnErrorAttribute : FilterAttribute, IExceptionFilter
    {
        public void OnException(ExceptionContext filterContext)
        {
            string controller = filterContext.Controller.GetType().Name;
            string action = filterContext.RouteData.Values["action"].ToString();
            string usernb = MyOwnData.MyNB().ToString();
            string username = MyOwnData.MyFullName();
            // Don't interfere if the exception is already handled
            if (filterContext.ExceptionHandled)
                return;
            // Let the next request know what went wrong
            filterContext.Controller.TempData["exception_type"] = filterContext.Exception.GetType().Name;
            filterContext.Controller.TempData["Controller"] = controller;
            filterContext.Controller.TempData["Action"] = action;
            filterContext.Controller.TempData["Error"] = filterContext.Exception.Message;
            filterContext.Controller.TempData["StackTrace"] = filterContext.Exception.StackTrace;
            // Set up a redirection to my global error handler
            filterContext.Result = new RedirectToRouteResult(new RouteValueDictionary(new { controller = "HandleException", action = "Index", area = "HandleExceptions" }));
            // Advise subsequent exception filters not to interfere
            // and stop ASP.NET from producing a "yellow screen of death"
            filterContext.ExceptionHandled = true;
            // Erase any output already generated
            filterContext.HttpContext.Response.Clear();
        }
    }
    public static class MyOwnData
    {      
        public static string GetCookie(string name)
        {
            string value = "غير محدد";
            if (HttpContext.Current.Request.Cookies[name] != null)
                return HttpContext.Current.Request.Cookies[name].Value;
            return value;
        }
        public static string GetMasterView()
        {
            long? repType = MyDataBase.GetReportType(MyOwnData.MyNB());
            if (repType == 1)
                return "~/Views/Shared/Print.Master";
            else if (repType == 2)
                return "~/Views/Shared/PrintXls.Master";
            else if (repType == 3)
                return "~/Views/Shared/PrintHtm.Master";
            return "~/Views/Shared/Print.Master";
        }

        public static string MyDB()
        {
            HttpCookie cookie;
            cookie = HttpContext.Current.Request.Cookies.Get("theDB");
            if (cookie != null)
                return cookie.Value;
            else
                return "ORACLE";
        }
        public static double MyYear()
        {
            HttpCookie cookie;
            cookie = HttpContext.Current.Request.Cookies.Get("MyYear");
            if (cookie != null)
                return double.Parse(cookie.Value);
            else
                return DateTime.Today.Year;
        }


        public static string GetMasterView(double repType)
        {

            if (repType == 1)
                return "~/Views/Shared/_PrintWord.cshtml";
            else if (repType == 2)
                return "~/Views/Shared/_PrintXls.cshtml";
            else if (repType == 3)
                return "~/Views/Shared/_PrintHtm.cshtml";
            return "~/Views/Shared/_PrintWord.cshtml";
        }

        public static string GetNarrowMasterView()
        {
            long? repType = MyDataBase.GetReportType(MyOwnData.MyNB());
            if (repType == 1)
                return "~/Views/Shared/PrintNarrow.Master";
            else if (repType == 2)
                return "~/Views/Shared/PrintXlsNarrow.Master";
            else if (repType == 3)
                return "~/Views/Shared/PrintHtmNarrow.Master";
            return "~/Views/Shared/PrintNarrow.Master";
        }

        public static string GetVerticalMasterView()
        {
            return "~/Views/Shared/_PrintVertical.cshtml";
        }
        public static void SetCookie(string name, string value)
        {
            if (HttpContext.Current.Response.Cookies.AllKeys.Contains(name))
            {
                HttpContext.Current.Response.Cookies[name].Value = value;
            }
            else
            {
                HttpCookie cookie = new HttpCookie(name);
                cookie.Value = value;
                System.Web.HttpContext.Current.Response.Cookies.Add(cookie);
            }
        }
        
        
        public static long? MyNB()
        {
            HttpCookie cookie;
            cookie = System.Web.HttpContext.Current.Request.Cookies.Get("MyNB");
            if (cookie != null)
            {
                return Int64.Parse(cookie.Value);
            }
            else
                return 0;
        }
        public static string GlobalMonth()
        {
            HttpCookie cookie;
            cookie = System.Web.HttpContext.Current.Request.Cookies.Get("GlobalMonth");
            if (cookie != null)
                return cookie.Value;
            else
                return "2013";
        }
        public static string MyAppPath
        {
            get
            {
                Uri uri = HttpContext.Current.Request.Url;
                return uri.Scheme + "://" + uri.Authority + "/Con_3roudIn";
            }
        }
        public static string MyFullName()
        {
            return MyDataBase.GetEmpFullName(MyNB());
        }
        public static string ConvertToEasternArabicNumerals(string input)
        {
            if (String.IsNullOrEmpty(input)) { return ""; }
            System.Text.UTF8Encoding utf8Encoder = new UTF8Encoding();
            System.Text.Decoder utf8Decoder = utf8Encoder.GetDecoder();
            System.Text.StringBuilder convertedChars = new System.Text.StringBuilder();
            char[] convertedChar = new char[1];
            byte[] bytes = new byte[] { 217, 160 };
            char[] inputCharArray = input.ToCharArray();
            foreach (char c in inputCharArray)
            {
                if (char.IsDigit(c))
                {
                    bytes[1] = Convert.ToByte(160 + char.GetNumericValue(c));
                    utf8Decoder.GetChars(bytes, 0, 2, convertedChar, 0);
                    convertedChars.Append(convertedChar[0]);
                }
                else
                {
                    convertedChars.Append(c);
                }
            }
            return convertedChars.ToString();
        }
    }
    public class RequireSSL : ActionFilterAttribute
    {
        public override void OnActionExecuting(ActionExecutingContext filterContext)
        {
            HttpRequestBase req = filterContext.HttpContext.Request;
            HttpResponseBase res = filterContext.HttpContext.Response;
            if (!req.IsSecureConnection && !req.IsLocal)
            {
                var builder = new UriBuilder(req.Url)
                {
                    Scheme = Uri.UriSchemeHttps,
                    Port = 443
                };
                res.Redirect(builder.Uri.ToString());
            }
            base.OnActionExecuting(filterContext);
        }
    }
    public static class ErrorString
    {
        public static string GetErrorString(int errCode)
        {
            switch (errCode)
            {
                case 00001: return "هذا السجل   موجود يرجى التدقيق وإعادة المحاولة";
                case 01400: return "هناك حقول واجبة الإدخال";
                case 01407: return "هناك حقول واجبة الإدخال";
                case 01722: return "يوجد خطأ بإدخال رقمي";
                case 01830: return "النص المدخل للتاريخ غير سليمة";
                case 02292: return "عملية الحذف غير ممكنة لوجود سجلات مرتبطة";
                case 01033: return "قاعدة المعطيات )أوراكل( قيد التشغيل. الرجاء الانتظار ثم إعادة المحاولة.";
                case 20001: return "لا يمكن إجراء العملية المطلوبة! تم قفل العام الحالي.";
                default: return "خطأ غير محدد يحمل الرقم /" + errCode.ToString() + "/";
            }
        }

        public static string GetErrorString(string errCode)
        {
            return errCode;
        }
    }

    
}


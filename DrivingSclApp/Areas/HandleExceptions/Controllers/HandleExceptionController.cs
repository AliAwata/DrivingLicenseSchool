using System.Web.Mvc;

using Oracle.ManagedDataAccess.Client;

namespace DrivingSclApp.Areas.HandleExceptions.Controllers
{

    public class HandleExceptionController : Controller
    {
        public void AddLog(string usernb, string username, string controller, string action, string message, string trace)
        {
            string sql = "INSERT INTO appmgr.LOG_ERRORS (USER_NB, USER_NAME, CONTROLLER, ACTION, MESSAGE, TRACE, APPLICATION)";
            sql += " VALUES (";
            sql += "'" + usernb + "', '" + username + "', '" + controller + "', :action , :message, :trace, '" + Resource.ProgID + "' )";
            OracleConnection conn = new OracleConnection(MyDataBase.GetConnectionString());
            try
            {
                conn.Open();
                OracleCommand cmd = new OracleCommand(sql, conn);
                cmd.Parameters.Add(new OracleParameter("action", action));
                cmd.Parameters.Add(new OracleParameter("message", message));
                cmd.Parameters.Add(new OracleParameter("trace", trace));
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
        public ActionResult Index()
        {
            string controller = TempData["Controller"].ToString();
            string action = TempData["Action"].ToString();
            //string exception_type= TempData["exception_type"];
            string usernb = MyOwnData.MyNB().ToString();
            string username = MyOwnData.MyFullName();
            // ToDo
            AddLog(MyOwnData.MyNB().ToString(), MyOwnData.MyFullName(), controller, action, TempData["Error"].ToString(), TempData["StackTrace"].ToString());
            return View();
        }
        public ActionResult MailTo()
        {
            return RedirectToAction("Index");
        }
    }
}

using DrivingSclApp.Areas.Schools.Data;
using DrivingSclData;
using Kendo.Mvc.Extensions;
using Kendo.Mvc.UI;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.IO;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using System.Web.UI.WebControls;

namespace DrivingSclApp.Areas.Schools.Controllers
{
    public class SchoolDocController : Controller
    {
        private DrivingSclEntity db = new DrivingSclEntity();
        private string FTPHost = "ftp://127.0.0.1/";
        private string FTPUsername = "FuryLord";
        private string FTPPassword = "AliXHamsa2009";
        public ActionResult Index()
        {
            return View();
        }
        public ActionResult Read([DataSourceRequest] DataSourceRequest request)
        {
            List<SchoolDocVm> Data = new List<SchoolDocVm>();
            Data = (from s in db.SCHOOLDOC
                    join scl in db.SCHOOL
                    on s.SCL_NB equals scl.NB
                    join typ in db.ZDOCTYPE
                    on s.TYPE_NB equals typ.NB
                    join usg in db.ZDOCUSAGE
                    on s.USAGE_NB equals usg.NB
                    select new SchoolDocVm
                    {
                        NB = s.NB,
                        SCL_NB = s.SCL_NB,
                        DAT = s.DAT,
                        NUM = s.NUM,
                        TYPE_NB = s.TYPE_NB,
                        ISSUER = s.ISSUER,
                        DESCRIB = s.DESCRIB,
                        USAGE_NB = s.USAGE_NB,
                        DOCFILE = s.DOCFILE,
                        NOTE = s.NOTE,
                        SCL_NAME = scl.SCLNAME,
                        TYPE_NAME = typ.DOCTYPE,
                        USAGE_NAME = usg.USAGETYPE
                    }).OrderBy(x => x.NB).ToList();
            return Json(Data.ToDataSourceResult(request), JsonRequestBehavior.AllowGet);
        }
        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult Create([DataSourceRequest] DataSourceRequest request, SCHOOLDOC model)
        {
            using (var transaction = db.Database.BeginTransaction())
            {
                if (ModelState.IsValid)
                {
                    try
                    {
                        model.NB = MyDataBase.GetSeqValue("GetIndexID");
                        db.SCHOOLDOC.Add(model);
                        db.SaveChanges();
                        transaction.Commit();
                    }
                    catch (Exception ex)
                    {
                        transaction.Rollback();
                        return Json(new { success = false, responseText = ex.InnerException.InnerException.Message }, JsonRequestBehavior.AllowGet);
                    }
                }
                else
                    return Json(new { success = false, responseText = "الرجاء التأكد من المدخلات!" }, JsonRequestBehavior.AllowGet);
            }
            return Json(new { success = true, responseText = "تمت الإضافة بنجاح" }, JsonRequestBehavior.AllowGet);
        }
        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult Update([DataSourceRequest] DataSourceRequest request, SCHOOLDOC model)
        {
            using (var transaction = db.Database.BeginTransaction())
            {
                if (ModelState.IsValid)
                {
                    try
                    {
                        db.SCHOOLDOC.Attach(model);
                        db.Entry(model).State = EntityState.Modified;
                        db.SaveChanges();
                        transaction.Commit();
                    }
                    catch (Exception ex)
                    {
                        transaction.Rollback();
                        return Json(new { success = false, responseText = ex.InnerException.InnerException.Message }, JsonRequestBehavior.AllowGet);
                    }
                }
                else
                    return Json(new { success = false, responseText = "الرجاء التأكد من المدخلات!" }, JsonRequestBehavior.AllowGet);
            }
            return Json(new { success = true, responseText = "تم التعديل بنجاح" }, JsonRequestBehavior.AllowGet);

        }
        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult Destroy([DataSourceRequest] DataSourceRequest request, SCHOOLDOC model)
        {
            using (var transaction = db.Database.BeginTransaction())
            {
                try
                {
                    db.SCHOOLDOC.Attach(model);
                    db.Entry(model).State = EntityState.Deleted;
                    db.SaveChanges();
                    transaction.Commit();
                }
                catch (Exception ex)
                {
                    transaction.Rollback();
                    return Json(new { success = false, responseText = ex.InnerException.InnerException.Message }, JsonRequestBehavior.AllowGet);
                }
            }
            return Json(new { success = true, responseText = "تم الحذف بنجاح" }, JsonRequestBehavior.AllowGet);
        }
        public ActionResult Documents(int id)
        {
            List<SchoolVM> Data = new List<SchoolVM>();
            Data = (from s in db.SCHOOL
                    join g in db.ZGOVERN
                    on s.GOV_NB equals g.NB
                    join c in db.ZCITY
                    on s.CTY_NB equals c.NB
                    join r in db.ZREGION
                    on s.REG_NB equals r.NB
                    join st in db.ZSCLSTATUS
                    on s.STS_NB equals st.NB
                    join sty in db.ZSCLTYPE
                    on s.ST_NB equals sty.NB
                    select new SchoolVM
                    {
                        NB = s.NB,
                        SCL_CODE = s.SCL_CODE,
                        SCLNAME = s.SCLNAME,
                        ST_NB = s.ST_NB,
                        GOV_NB = s.GOV_NB,
                        COMREG_NO = s.COMREG_NO,
                        COMREG_DATE = s.COMREG_DATE,
                        COMREG_TYP = s.COMREG_TYP,
                        COMREG_GOV = s.COMREG_GOV,
                        CTY_NB = s.CTY_NB,
                        REG_NB = s.REG_NB,
                        ADDRESS = s.ADDRESS,
                        STS_NB = s.STS_NB,
                        GOV_NAME = g.NAME,
                        CTY_NAME = c.NAME,
                        REG_NAME = r.NAME,
                        STS_NAME = st.STSNAME,
                        STY_NAME = sty.TYPNAME
                    }).Where(s => s.NB == id).OrderBy(x => x.NB).ToList();
            ViewData["ID"] = id;
            return PartialView("_SchoolDoc", Data);
        }
        public ActionResult DocumentsById([DataSourceRequest] DataSourceRequest request,int id)
        {
            List<SchoolDocVm> Data = new List<SchoolDocVm>();
            Data = (from s in db.SCHOOLDOC
                    join scl in db.SCHOOL
                    on s.SCL_NB equals scl.NB
                    join typ in db.ZDOCTYPE
                    on s.TYPE_NB equals typ.NB
                    join usg in db.ZDOCUSAGE
                    on s.USAGE_NB equals usg.NB
                    select new SchoolDocVm
                    {
                        NB = s.NB,
                        SCL_NB = s.SCL_NB,
                        DAT = s.DAT,
                        NUM = s.NUM,
                        TYPE_NB = s.TYPE_NB,
                        ISSUER = s.ISSUER,
                        DESCRIB = s.DESCRIB,
                        USAGE_NB = s.USAGE_NB,
                        DOCFILE = s.DOCFILE,
                        NOTE = s.NOTE,
                        SCL_NAME = scl.SCLNAME,
                        TYPE_NAME = typ.DOCTYPE,
                        USAGE_NAME = usg.USAGETYPE
                    }).Where(x => x.SCL_NB == id).OrderBy(x => x.NB).ToList();
            return Json(Data.ToDataSourceResult(request), JsonRequestBehavior.AllowGet);
        }
        public string SaveDocument(HttpPostedFileBase txtDocFile)
        {
            UploadToFTP(txtDocFile);
            return "Done!";
        }
        public void UploadToFTP(HttpPostedFileBase uploadfile)
        {
            var uploadurl = FTPHost;
            var uploadfilename = uploadfile.FileName;
            var username = FTPUsername;
            var password = FTPPassword;
            Stream streamObj = uploadfile.InputStream;
            byte[] buffer = new byte[uploadfile.ContentLength];
            streamObj.Read(buffer, 0, buffer.Length);
            streamObj.Close();
            streamObj = null;
            string ftpurl = String.Format("{0}/{1}", uploadurl, uploadfilename);
            var requestObj = FtpWebRequest.Create(ftpurl) as FtpWebRequest;
            requestObj.Method = WebRequestMethods.Ftp.UploadFile;
            requestObj.Credentials = new NetworkCredential(username, password);
            Stream requestStream = requestObj.GetRequestStream();
            requestStream.Write(buffer, 0, buffer.Length);
            requestStream.Flush();
            requestStream.Close();
            requestObj = null;
        }
        public void FTPViewPDF(HttpPostedFileBase sender, EventArgs e)
        {
            string fileName = sender.FileName;

            //FTP Server URL.
            string ftp = FTPHost;

            //FTP Folder name. Leave blank if you want to Download file from root folder.
            string ftpFolder = "/";

            try
            {
                //Create FTP Request.
                FtpWebRequest request = (FtpWebRequest)WebRequest.Create(ftp + ftpFolder + fileName);
                request.Method = WebRequestMethods.Ftp.DownloadFile;

                //Enter FTP Server credentials.
                request.Credentials = new NetworkCredential(FTPUsername, FTPPassword);
                request.UsePassive = true;
                request.UseBinary = true;
                request.EnableSsl = false;

                //Fetch the Response and read it into a MemoryStream object.
                FtpWebResponse webResponse = (FtpWebResponse)request.GetResponse();
                using (MemoryStream stream = new MemoryStream())
                {
                    //Download the File.
                    CopyStream(webResponse.GetResponseStream(), stream);
                    byte[] bytes = stream.ToArray();

                    Response.Cache.SetCacheability(HttpCacheability.NoCache);
                    Response.ContentType = "application/pdf";
                    Response.BinaryWrite(bytes);
                    Response.Flush();
                    Response.End();
                }
            }
            catch (WebException ex)
            {
                throw new Exception((ex.Response as FtpWebResponse).StatusDescription);
            }
        }
        public static void CopyStream(Stream input, Stream output)
        {
            byte[] buffer = new byte[4096];
            while (true)
            {
                int read = input.Read(buffer, 0, buffer.Length);
                if (read <= 0)
                {
                    return;
                }
                output.Write(buffer, 0, read);
            }
        }
    }
}

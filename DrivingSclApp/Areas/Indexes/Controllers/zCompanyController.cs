using DrivingSclApp.Areas.Indexes.Data;
using DrivingSclData;
using Kendo.Mvc.Extensions;
using Kendo.Mvc.UI;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Web.Mvc;

namespace DrivingSclApp.Areas.Indexes.Controllers
{
    [Authorize, Audit, CanDoIt, NoCache, RedirectOnError]
    public class zCompanyController : Controller
    {
        private DrivingSclEntity db = new DrivingSclEntity();
        public ActionResult Index()
        {
            return View();
        }
        public ActionResult IndexDetails(int id)
        {
            List<CompanyVM> Data = new List<CompanyVM>();
            Data = (from a in db.ZCOMPANY
                    join g in db.ZGOVERN
                    on a.COMREG_GOV equals g.NB
                    join c in db.ZCITY
                    on a.CTY_NB equals c.NB
                    join r in db.ZREGION
                    on a.REG_NB equals r.NB
                    select new CompanyVM
                    {
                        NB = a.NB,
                        COMPNAME = a.COMPNAME,
                        COMREG_NO = a.COMREG_NO,
                        COMREG_DATE = a.COMREG_DATE,
                        COMREG_TYP = a.COMREG_TYP,
                        COMREG_GOV = a.COMREG_GOV,
                        CTY_NB = a.CTY_NB,
                        REG_NB = a.REG_NB,
                        ADDRESS = a.ADDRESS,
                        PHONE1 = a.PHONE1,
                        PHONE2 = a.PHONE2,
                        MOBILE = a.MOBILE,
                        FAX = a.FAX,
                        NOTE = a.NOTE,
                        GovName = g.NAME,
                        CityName = c.NAME,
                        RegionName = r.NAME
                    }).Where(x => x.NB == id).OrderBy(x => x.NB).ToList();
            ViewData["ID"] = id;
            return View(Data);
        }
        public ActionResult Read([DataSourceRequest] DataSourceRequest request)
        {
            List<CompanyVM> Data = new List<CompanyVM>();
            Data = (from a in db.ZCOMPANY
                    join g in db.ZGOVERN
                    on a.COMREG_GOV equals g.NB
                    join c in db.ZCITY
                    on a.CTY_NB equals c.NB
                    join r in db.ZREGION
                    on a.REG_NB equals r.NB
                    select new CompanyVM
                    {
                        NB = a.NB,
                        COMPNAME = a.COMPNAME,
                        COMREG_NO = a.COMREG_NO,
                        COMREG_DATE = a.COMREG_DATE,
                        COMREG_TYP = a.COMREG_TYP,
                        COMREG_GOV = a.COMREG_GOV,
                        CTY_NB = a.CTY_NB,
                        REG_NB = a.REG_NB,
                        ADDRESS = a.ADDRESS,
                        PHONE1 = a.PHONE1,
                        PHONE2 = a.PHONE2,
                        MOBILE = a.MOBILE,
                        FAX = a.FAX,
                        NOTE = a.NOTE,
                        GovName = g.NAME,
                        CityName = c.NAME,
                        RegionName = r.NAME
                    }).OrderBy(x => x.NB).ToList();
            return Json(Data.ToDataSourceResult(request), JsonRequestBehavior.AllowGet);
        }
        public ActionResult ReadByFilter([DataSourceRequest] DataSourceRequest request
            ,string comp_name = "", string comreg_no = "", string comreg_typ = ""
            ,string govern = "", string city = "", string region = "")
        {
            List<CompanyVM> Data = new List<CompanyVM>();
            Data = (from a in db.ZCOMPANY
                    join g in db.ZGOVERN
                    on a.COMREG_GOV equals g.NB
                    join c in db.ZCITY
                    on a.CTY_NB equals c.NB
                    join r in db.ZREGION
                    on a.REG_NB equals r.NB
                    select new CompanyVM
                    {
                        NB = a.NB,
                        COMPNAME = a.COMPNAME,
                        COMREG_NO = a.COMREG_NO,
                        COMREG_DATE = a.COMREG_DATE,
                        COMREG_TYP = a.COMREG_TYP,
                        COMREG_GOV = a.COMREG_GOV,
                        CTY_NB = a.CTY_NB,
                        REG_NB = a.REG_NB,
                        ADDRESS = a.ADDRESS,
                        PHONE1 = a.PHONE1,
                        PHONE2 = a.PHONE2,
                        MOBILE = a.MOBILE,
                        FAX = a.FAX,
                        NOTE = a.NOTE,
                        GovName = g.NAME,
                        CityName = c.NAME,
                        RegionName = r.NAME
                    }).OrderBy(x => x.NB).ToList();
            if(comp_name != "")
            {
                Data = Data.Where(x => x.COMPNAME == comp_name).ToList();
            }
            if (comreg_no != "")
            {
                Data = Data.Where(x => x.COMREG_NO == comreg_no).ToList();
            }
            if (comreg_typ != "")
            {
                Data = Data.Where(x => x.COMREG_TYP == comreg_typ).ToList();
            }
            if (govern != "")
            {
                Data = Data.Where(x => x.COMREG_GOV.ToString() == govern).ToList();
            }
            if (city != "")
            {
                Data = Data.Where(x => x.CTY_NB.ToString() == city).ToList();
            }
            if (region != "")
            {
                Data = Data.Where(x => x.REG_NB.ToString() == region).ToList();
            }
            return Json(Data, JsonRequestBehavior.AllowGet);
        }
        public ActionResult ReadById([DataSourceRequest] DataSourceRequest request, int id)
        {
            List<CompanyVM> Data = new List<CompanyVM>();
            Data = (from a in db.ZCOMPANY
                    join g in db.ZGOVERN
                    on a.COMREG_GOV equals g.NB
                    join c in db.ZCITY
                    on a.CTY_NB equals c.NB
                    join r in db.ZREGION
                    on a.REG_NB equals r.NB
                    select new CompanyVM
                    {
                        NB = a.NB,
                        COMPNAME = a.COMPNAME,
                        COMREG_NO = a.COMREG_NO,
                        COMREG_DATE = a.COMREG_DATE,
                        COMREG_TYP = a.COMREG_TYP,
                        COMREG_GOV = a.COMREG_GOV,
                        CTY_NB = a.CTY_NB,
                        REG_NB = a.REG_NB,
                        ADDRESS = a.ADDRESS,
                        PHONE1 = a.PHONE1,
                        PHONE2 = a.PHONE2,
                        MOBILE = a.MOBILE,
                        FAX = a.FAX,
                        NOTE = a.NOTE,
                        GovName = g.NAME,
                        CityName = c.NAME,
                        RegionName = r.NAME
                    }).Where(x => x.NB == id).OrderBy(x => x.NB).ToList();
            return Json(Data.ToDataSourceResult(request), JsonRequestBehavior.AllowGet);
        }
        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult Create([DataSourceRequest] DataSourceRequest request, ZCOMPANY model)
        {
            using (var transaction = db.Database.BeginTransaction())
            {
                if (ModelState.IsValid)
                {
                    try
                    {
                        model.NB = MyDataBase.GetSeqValue("GetIndexID");
                        db.ZCOMPANY.Add(model);
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
        public ActionResult Create_GetNb([DataSourceRequest] DataSourceRequest request, ZCOMPANY model)
        {
            using (var transaction = db.Database.BeginTransaction())
            {
                if (ModelState.IsValid)
                {
                    try
                    {
                        model.NB = MyDataBase.GetSeqValue("GetIndexID");
                        db.ZCOMPANY.Add(model);
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
            return Json(model.NB, JsonRequestBehavior.AllowGet);
        }
        public ActionResult Create_GetScl([DataSourceRequest] DataSourceRequest request, ZCOMPANY model)
        {
            using (var transaction = db.Database.BeginTransaction())
            {
                if (ModelState.IsValid)
                {
                    try
                    {
                        model.NB = MyDataBase.GetSeqValue("GetIndexID");
                        db.ZCOMPANY.Add(model);
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
            return Json(model, JsonRequestBehavior.AllowGet);
        }
        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult Update([DataSourceRequest] DataSourceRequest request, ZCOMPANY model)
        {
            using (var transaction = db.Database.BeginTransaction())
            {
                if (ModelState.IsValid)
                {
                    try
                    {
                        db.ZCOMPANY.Attach(model);
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
        public ActionResult Destroy([DataSourceRequest] DataSourceRequest request, ZCOMPANY model)
        {
            using (var transaction = db.Database.BeginTransaction())
            {
                try
                {
                    db.ZCOMPANY.Attach(model);
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
        public JsonResult validation(string param1)
        {
            string ss = MyDataBase.RunQuery("select count(*) from ZCOMPANY where Compname='" + param1 + "'");
            if (Convert.ToInt32(ss) >= 1)
            {
                return Json(new { success = false, responseText = "الاسم موجود بالفعل" }, JsonRequestBehavior.AllowGet);
            }
            return Json(new { success = true, responseText = "تمت عملية الحفظ بنجاح" }, JsonRequestBehavior.AllowGet);
        }
    }
}
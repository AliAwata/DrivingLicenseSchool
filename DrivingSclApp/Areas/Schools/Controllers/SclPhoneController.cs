using DrivingSclApp.Areas.Indexes.Controllers;
using DrivingSclApp.Areas.Schools.Data;
using DrivingSclData;
using Kendo.Mvc.Extensions;
using Kendo.Mvc.UI;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Web.Mvc;

namespace DrivingSclApp.Areas.Schools.Controllers
{
    public class SclPhoneController : Controller
    {
        private DrivingSclEntity db = new DrivingSclEntity();
        private CodesController codes = new CodesController();
        public ActionResult Index()
        {
            return View();
        }
        public ActionResult Read([DataSourceRequest] DataSourceRequest request)
        {
            List<SclPhoneVM> Data = new List<SclPhoneVM>();
            Data = (from p in db.SCLPHONE
                    join s in db.SCHOOL
                    on p.SCL_NB equals s.NB
                    select new SclPhoneVM
                    {
                        NB = p.NB,
                        SCL_NB = p.SCL_NB,
                        PHONE_NO = p.PHONE_NO,
                        PHONE_TYP = p.PHONE_TYP,
                        SCL_NAME = s.SCLNAME
                    }).ToList();
            foreach(var item in Data)
            {
                item.PHONE_TYPE = codes.GetPhoneTypeName(item.PHONE_TYP);
            }
            return Json(Data.ToDataSourceResult(request), JsonRequestBehavior.AllowGet);
        }
        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult Create([DataSourceRequest] DataSourceRequest request, SCLPHONE model)
        {
            using (var transaction = db.Database.BeginTransaction())
            {
                if (ModelState.IsValid)
                {
                    try
                    {
                        model.NB = MyDataBase.GetSeqValue("GetIndexID");
                        db.SCLPHONE.Add(model);
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
        public ActionResult Update([DataSourceRequest] DataSourceRequest request, SCLPHONE model)
        {
            using (var transaction = db.Database.BeginTransaction())
            {
                if (ModelState.IsValid)
                {
                    try
                    {
                        db.SCLPHONE.Attach(model);
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
        public ActionResult Destroy([DataSourceRequest] DataSourceRequest request, SCLPHONE model)
        {
            using (var transaction = db.Database.BeginTransaction())
            {
                try
                {
                    db.SCLPHONE.Attach(model);
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
        public ActionResult Phones(int id)
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
                    }).Where(x => x.NB == id).ToList();
            ViewData["ID"] = id;
            return PartialView("_SclPhone", Data);
        }
        public ActionResult PhonesBySclNb([DataSourceRequest] DataSourceRequest request, int id)
        {
            List<SclPhoneVM> Data = new List<SclPhoneVM>();
            Data = (from p in db.SCLPHONE
                    join s in db.SCHOOL
                    on p.SCL_NB equals s.NB
                    select new SclPhoneVM
                    {
                        NB = p.NB,
                        SCL_NB = p.SCL_NB,
                        PHONE_NO = p.PHONE_NO,
                        PHONE_TYP = p.PHONE_TYP,
                        SCL_NAME = s.SCLNAME
                    }).Where(x => x.SCL_NB == id).ToList();
            foreach (var item in Data)
            {
                item.PHONE_TYPE = codes.GetPhoneTypeName(item.PHONE_TYP);
            }
            ViewData["ID"] = id;
            return Json(Data.ToDataSourceResult(request), JsonRequestBehavior.AllowGet);
        }
    }
}
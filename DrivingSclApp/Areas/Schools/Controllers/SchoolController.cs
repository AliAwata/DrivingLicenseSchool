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
    public class SchoolController : Controller
    {
        private DrivingSclEntity db = new DrivingSclEntity();
        public ActionResult Index()
        {
            return View();
        }
        public ActionResult IndexDetails(int id)
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
                    }).Where(x => x.NB == id).OrderBy(x => x.NB).ToList();
            ViewData["ID"] = id;
            return View(Data);
        }
        public ActionResult Read([DataSourceRequest] DataSourceRequest request)
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
                    }).OrderBy(x => x.NB).ToList();
            return Json(Data.ToDataSourceResult(request), JsonRequestBehavior.AllowGet);
        }
        public ActionResult ReadByFilter(string scl_name = "", string scl_code = "", string scl_typ = "",
            string govern = "", string city = "", string region  = "")
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
                    }).OrderBy(x => x.NB).ToList();
            if(scl_name != "")
            {
                Data = Data.Where(x => x.SCLNAME.Contains(scl_name)).ToList();
            }
            if (scl_code != "")
            {
                Data = Data.Where(x => x.SCL_CODE == scl_code).ToList();
            }
            if (scl_typ != "")
            {
                Data = Data.Where(x => x.ST_NB.ToString() == scl_typ).ToList();
            }
            if (govern != "")
            {
                Data = Data.Where(x => x.GOV_NB.ToString() == govern).ToList();
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
            return Json(Data.ToDataSourceResult(request), JsonRequestBehavior.AllowGet);
        }
        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult Create([DataSourceRequest] DataSourceRequest request, SCHOOL model)
        {
            using (var transaction = db.Database.BeginTransaction())
            {
                if (ModelState.IsValid)
                {
                    try
                    {
                        model.NB = MyDataBase.GetSeqValue("GetIndexID");
                        model.STS_NB = 1;
                        db.SCHOOL.Add(model);
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
        public ActionResult Update([DataSourceRequest] DataSourceRequest request, SCHOOL model)
        {
            using (var transaction = db.Database.BeginTransaction())
            {
                if (ModelState.IsValid)
                {
                    try
                    {
                        db.SCHOOL.Attach(model);
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
        public ActionResult Destroy([DataSourceRequest] DataSourceRequest request, SCHOOL model)
        {
            using (var transaction = db.Database.BeginTransaction())
            {
                try
                {
                    db.SCHOOL.Attach(model);
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
            string ss = MyDataBase.RunQuery("select count(*) from SCHOOLOWNER where SCLNAME='" + param1 + "'");
            if (Convert.ToInt32(ss) >= 1)
            {
                return Json(new { success = false, responseText = "الاسم موجود بالفعل" }, JsonRequestBehavior.AllowGet);
            }
            return Json(new { success = true, responseText = "تمت عملية الحفظ بنجاح" }, JsonRequestBehavior.AllowGet);
        }
    }
}
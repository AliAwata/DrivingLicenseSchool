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
    public class SchoolTrainerController : Controller
    {
        private DrivingSclEntity db = new DrivingSclEntity();
        private CodesController codes = new CodesController();
        public ActionResult Index()
        {
            return View();
        }
        public ActionResult Read([DataSourceRequest] DataSourceRequest request)
        {
            List<SchoolTrainerVM> Data = new List<SchoolTrainerVM>();
            Data = (from t in db.SCHOOLTRAINER
                    join p in db.ZPERSON
                    on t.PRS_NB equals p.NB
                    join s in db.SCHOOL
                    on t.SCL_NB equals s.NB
                    join typ in db.ZTRAINERTYPE
                    on t.TYP_NB equals typ.NB
                    select new SchoolTrainerVM
                    {
                        NB = t.NB,
                        PRS_NB = t.PRS_NB,
                        SCL_NB = t.SCL_NB,
                        TYP_NB = t.TYP_NB,
                        DIPLOM = t.DIPLOM,
                        LICENSENO = t.LICENSENO,
                        LICENSEDATE = t.LICENSEDATE,
                        LICENSEFROM = t.LICENSEFROM,
                        SCL_NAME = s.SCLNAME,
                        TYP_NAME = typ.NAME
                    }).OrderBy(x => x.NB).ToList();
            foreach(var item in Data)
            {
                item.PRS_NAME = codes.GetPrsFullName(item.PRS_NB);
            }
            return Json(Data.ToDataSourceResult(request), JsonRequestBehavior.AllowGet);
        }
        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult Create([DataSourceRequest] DataSourceRequest request, SCHOOLTRAINER model)
        {
            using (var transaction = db.Database.BeginTransaction())
            {
                if (ModelState.IsValid)
                {
                    try
                    {
                        db.SCHOOLTRAINER.Add(model);
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
        public ActionResult Update([DataSourceRequest] DataSourceRequest request, SCHOOLTRAINER model)
        {
            using (var transaction = db.Database.BeginTransaction())
            {
                if (ModelState.IsValid)
                {
                    try
                    {
                        db.SCHOOLTRAINER.Attach(model);
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
        public ActionResult Destroy([DataSourceRequest] DataSourceRequest request, SCHOOLTRAINER model)
        {
            using (var transaction = db.Database.BeginTransaction())
            {
                try
                {
                    db.SCHOOLTRAINER.Attach(model);
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
        public ActionResult Trainers(int id)
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
            return PartialView("_SchoolTrainer", Data);
        }
        public ActionResult TrainersBySclNb([DataSourceRequest] DataSourceRequest request, int id)
        {
            List<SchoolTrainerVM> Data = new List<SchoolTrainerVM>();
            Data = (from t in db.SCHOOLTRAINER
                    join p in db.ZPERSON
                    on t.PRS_NB equals p.NB
                    join s in db.SCHOOL
                    on t.SCL_NB equals s.NB
                    join typ in db.ZTRAINERTYPE
                    on t.TYP_NB equals typ.NB
                    select new SchoolTrainerVM
                    {
                        NB = t.NB,
                        PRS_NB = t.PRS_NB,
                        SCL_NB = t.SCL_NB,
                        TYP_NB = t.TYP_NB,
                        DIPLOM = t.DIPLOM,
                        LICENSENO = t.LICENSENO,
                        LICENSEDATE = t.LICENSEDATE,
                        LICENSEFROM = t.LICENSEFROM,
                        SCL_NAME = s.SCLNAME,
                        TYP_NAME = typ.NAME
                    }).Where(x => x.SCL_NB == id).OrderBy(x => x.PRS_NB).ToList();
            foreach (var item in Data)
            {
                item.PRS_NAME = codes.GetPrsFullName(item.PRS_NB);
            }
            return Json(Data.ToDataSourceResult(request), JsonRequestBehavior.AllowGet);
        }

    }

}
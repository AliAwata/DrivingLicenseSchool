using DrivingSclApp.Areas.Indexes.Data;
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
    public class SchoolVehiclesController : Controller
    {
        private DrivingSclEntity db = new DrivingSclEntity();
        public ActionResult Index()
        {
            return View();
        }
        public ActionResult Read([DataSourceRequest] DataSourceRequest request)
        {
            List<SchoolVehiclesVM> Data = new List<SchoolVehiclesVM>();
            Data = (from v in db.SCHOOLVEHICLE
                    join p in db.ZPERSON
                    on v.PRS_NB equals p.NB
                    join s in db.SCHOOL
                    on v.SCL_NB equals s.NB
                    join g in db.ZGOVERN
                    on v.GOV_NB equals g.NB
                    join c in db.ZVEHICLECLASS
                    on v.CLS_NB equals c.NB
                    select new SchoolVehiclesVM
                    {
                        NB = v.NB,
                        SCL_NB = v.SCL_NB,
                        GOV_NB = v.GOV_NB,
                        BOARDNO = v.BOARDNO,
                        VIN = v.VIN,
                        ENGINENO = v.ENGINENO,
                        CLS_NB = v.CLS_NB,
                        COLOR = v.COLOR,
                        WIDTH = v.WIDTH,
                        LENG = v.LENG,
                        ENGINESIZE = v.ENGINESIZE,
                        PRODYEAR = v.PRODYEAR,
                        REGYEAR = v.REGYEAR,
                        BRAND = v.BRAND,
                        MODELNO = v.MODELNO,
                        SEATS = v.SEATS,
                        PRS_NB = v.PRS_NB,
                        LICENSEEXPIRYDATE = v.LICENSEEXPIRYDATE,
                        SCL_NAME = s.SCLNAME,
                        PRS_NAME = p.FNAME + " " + p.LNAME,
                        GOV_NAME = g.NAME,
                        CLS_NAME = c.NAME
                    }).OrderBy(x => x.NB).ToList();
            return Json(Data.ToDataSourceResult(request), JsonRequestBehavior.AllowGet);
        }
        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult Create([DataSourceRequest] DataSourceRequest request, SCHOOLVEHICLE model)
        {
            using (var transaction = db.Database.BeginTransaction())
            {
                if (ModelState.IsValid)
                {
                    try
                    {
                        model.VIN = model.VIN.ToUpper();
                        model.ENGINENO = model.ENGINENO.ToUpper();
                        model.NB = MyDataBase.GetSeqValue("GetIndexID");
                        db.SCHOOLVEHICLE.Add(model);
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
        public ActionResult Update([DataSourceRequest] DataSourceRequest request, SCHOOLVEHICLE model)
        {
            using (var transaction = db.Database.BeginTransaction())
            {
                if (ModelState.IsValid)
                {
                    try
                    {
                        model.VIN = model.VIN.ToUpper();
                        model.ENGINENO = model.ENGINENO.ToUpper();
                        db.SCHOOLVEHICLE.Attach(model);
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
        public ActionResult Destroy([DataSourceRequest] DataSourceRequest request, SCHOOLVEHICLE model)
        {
            using (var transaction = db.Database.BeginTransaction())
            {
                try
                {
                    db.SCHOOLVEHICLE.Attach(model);
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
        public ActionResult Vehicles(int id)
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
            return PartialView("_SchoolVehicles", Data);
        }
        public ActionResult VehiclesBySclNb([DataSourceRequest] DataSourceRequest request, int id)
        {
            List<SchoolVehiclesVM> Data = new List<SchoolVehiclesVM>();
            Data = (from v in db.SCHOOLVEHICLE
                    join p in db.ZPERSON
                    on v.PRS_NB equals p.NB
                    join s in db.SCHOOL
                    on v.SCL_NB equals s.NB
                    join g in db.ZGOVERN
                    on v.GOV_NB equals g.NB
                    join c in db.ZVEHICLECLASS
                    on v.CLS_NB equals c.NB
                    select new SchoolVehiclesVM
                    {
                        NB = v.NB,
                        SCL_NB = v.SCL_NB,
                        GOV_NB = v.GOV_NB,
                        BOARDNO = v.BOARDNO,
                        VIN = v.VIN,
                        ENGINENO = v.ENGINENO,
                        CLS_NB = v.CLS_NB,
                        COLOR = v.COLOR,
                        WIDTH = v.WIDTH,
                        LENG = v.LENG,
                        ENGINESIZE = v.ENGINESIZE,
                        PRODYEAR = v.PRODYEAR,
                        REGYEAR = v.REGYEAR,
                        BRAND = v.BRAND,
                        MODELNO = v.MODELNO,
                        SEATS = v.SEATS,
                        PRS_NB = v.PRS_NB,
                        LICENSEEXPIRYDATE = v.LICENSEEXPIRYDATE,
                        SCL_NAME = s.SCLNAME,
                        PRS_NAME = p.FNAME,
                        GOV_NAME = g.NAME,
                        CLS_NAME = c.NAME
                    }).Where(x => x.SCL_NB == id).OrderBy(x => x.NB).ToList();
            return Json(Data.ToDataSourceResult(request), JsonRequestBehavior.AllowGet);
        }
        public ActionResult VehiclesFilter(string school = "", string person = "", string govern = ""
            , string cls = "", string boardno = "")
        {
            List<SchoolVehiclesVM> Data = new List<SchoolVehiclesVM>();
            Data = (from v in db.SCHOOLVEHICLE
                    join p in db.ZPERSON
                    on v.PRS_NB equals p.NB
                    join s in db.SCHOOL
                    on v.SCL_NB equals s.NB
                    join g in db.ZGOVERN
                    on v.GOV_NB equals g.NB
                    join c in db.ZVEHICLECLASS
                    on v.CLS_NB equals c.NB
                    select new SchoolVehiclesVM
                    {
                        NB = v.NB,
                        SCL_NB = v.SCL_NB,
                        GOV_NB = v.GOV_NB,
                        BOARDNO = v.BOARDNO,
                        VIN = v.VIN,
                        ENGINENO = v.ENGINENO,
                        CLS_NB = v.CLS_NB,
                        COLOR = v.COLOR,
                        WIDTH = v.WIDTH,
                        LENG = v.LENG,
                        ENGINESIZE = v.ENGINESIZE,
                        PRODYEAR = v.PRODYEAR,
                        REGYEAR = v.REGYEAR,
                        BRAND = v.BRAND,
                        MODELNO = v.MODELNO,
                        SEATS = v.SEATS,
                        PRS_NB = v.PRS_NB,
                        LICENSEEXPIRYDATE = v.LICENSEEXPIRYDATE,
                        SCL_NAME = s.SCLNAME,
                        PRS_NAME = p.FNAME,
                        GOV_NAME = g.NAME,
                        CLS_NAME = c.NAME
                    }).OrderBy(x => x.NB).ToList();
            if(school != "")
            {
                Data = Data.Where(x => x.SCL_NB.ToString() == school).ToList();
            }
            if (person != "")
            {
                Data = Data.Where(x => x.PRS_NB.ToString() == person).ToList();
            }
            if (govern != "")
            {
                Data = Data.Where(x => x.GOV_NB.ToString() == govern).ToList();
            }
            if (cls != "")
            {
                Data = Data.Where(x => x.CLS_NB.ToString() == cls).ToList();
            }
            if (boardno != "")
            {
                Data = Data.Where(x => x.BOARDNO == school).ToList();
            }
            return Json(Data, JsonRequestBehavior.AllowGet);
        }
        public ActionResult SchoolFilter(string scl_name = "", string scl_code = "", string comreg_no = ""
            , string govern = "", string city = "", string region = "")
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
            if (comreg_no != "")
            {
                Data = Data.Where(x => x.COMREG_NO == comreg_no).ToList();
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
    }
}
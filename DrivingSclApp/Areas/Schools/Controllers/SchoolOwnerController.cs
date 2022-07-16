using DrivingSclApp.Areas.Indexes.Controllers;
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
    public class SchoolOwnerController : Controller
    {
        private DrivingSclEntity db = new DrivingSclEntity();
        private CodesController codes = new CodesController();
        public ActionResult Index()
        {
            return View();
        }
        public ActionResult Read([DataSourceRequest] DataSourceRequest request)
        {
            List<SchoolOwnerVM> Data = new List<SchoolOwnerVM>();
            Data = (from so in db.SCHOOLOWNER
                    join s in db.SCHOOL
                    on so.SCL_NB equals s.NB
                    join o in db.ZOWNERTYP
                    on so.OT_NB equals o.NB
                    select new SchoolOwnerVM
                    {
                        NB = so.NB,
                        SCL_NB = so.SCL_NB,
                        OT_NB = so.OT_NB,
                        ONR_NB = so.ONR_NB,
                        NOTE = so.NOTE,
                        OwnerName = o.OTNAME,
                        SchoolName = s.SCLNAME
                    }).OrderBy(x => x.NB).ToList();
            return Json(Data.ToDataSourceResult(request), JsonRequestBehavior.AllowGet);
        }
        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult Create([DataSourceRequest] DataSourceRequest request, SCHOOLOWNER model)
        {
            using (var transaction = db.Database.BeginTransaction())
            {
                if (ModelState.IsValid)
                {
                    try
                    {
                        model.NB = MyDataBase.GetSeqValue("GetIndexID");
                        db.SCHOOLOWNER.Add(model);
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
        public ActionResult Update([DataSourceRequest] DataSourceRequest request, SCHOOLOWNER model)
        {
            using (var transaction = db.Database.BeginTransaction())
            {
                if (ModelState.IsValid)
                {
                    try
                    {
                        db.SCHOOLOWNER.Attach(model);
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
        public ActionResult Destroy([DataSourceRequest] DataSourceRequest request, SCHOOLOWNER model)
        {
            using (var transaction = db.Database.BeginTransaction())
            {
                try
                {
                    db.SCHOOLOWNER.Attach(model);
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
        public ActionResult Owner(int id)
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
            return PartialView("_SchoolOwner", Data);
        }
        public ActionResult OwnerBySclNb([DataSourceRequest] DataSourceRequest request, int id)
        {
            List<SchoolOwnerVM> Data = new List<SchoolOwnerVM>();
            Data = (from so in db.SCHOOLOWNER
                    join s in db.SCHOOL
                    on so.SCL_NB equals s.NB
                    join o in db.ZOWNERTYP
                    on so.OT_NB equals o.NB
                    select new SchoolOwnerVM
                    {
                        NB = so.NB,
                        SCL_NB = so.SCL_NB,
                        OT_NB = so.OT_NB,
                        ONR_NB = so.ONR_NB,
                        NOTE = so.NOTE,
                        SchoolName = s.SCLNAME,
                        OwnerTypName = o.OTNAME
                    }).Where(x => x.SCL_NB == id).OrderBy(x => x.NB).ToList();
            foreach(var item in Data)
            {
                if(item.OT_NB == 1)
                {
                    item.OwnerName = codes.GetPrsFullName(item.ONR_NB);
                } else if (item.OT_NB == 2)
                {
                    var company = db.ZCOMPANY.Find(item.ONR_NB);
                    item.OwnerName = company.COMPNAME;
                }
            }
            ViewData["ID"] = id;
            return Json(Data.ToDataSourceResult(request), JsonRequestBehavior.AllowGet);
        }
        public ActionResult PersonFilter(string fname = "", string lname = "", string natno = "",
            string father = "", string mother = "", string bdatey = "")
        {
            List<PersonVM> Data = new List<PersonVM>();
            Data = (from a in db.ZPERSON
                    join b in db.ZNATION
                    on a.NAT equals b.NB
                    join c in db.ZPRSTYPE
                    on a.TYP equals c.NB
                    select new PersonVM
                    {
                        NB = a.NB,
                        NATNO = a.NATNO,
                        FNAME = a.FNAME,
                        LNAME = a.LNAME,
                        FATHER = a.FATHER,
                        MOTHER = a.MOTHER,
                        CIVILLOC = a.CIVILLOC,
                        ACTADDRESS = a.ACTADDRESS,
                        ADDRESS = a.ADDRESS,
                        PHONE = a.PHONE,
                        MOBILE = a.MOBILE,
                        BDATED = a.BDATED,
                        BDATEM = a.BDATEM,
                        BDATEY = a.BDATEY,
                        BDATE = a.BDATE,
                        IDCARDNO = a.IDCARDNO,
                        IDCARDDAT = a.IDCARDDAT,
                        BPLACE = a.BPLACE,
                        ALAMANA = a.ALAMANA,
                        SEX = a.SEX,
                        TYP = a.TYP,
                        NAT = a.NAT,
                        NationName = b.NATION,
                        PerType = c.TYPNAME
                    }).OrderBy(x => x.NB).ToList();
            foreach (var item in Data)
            {
                if (item.SEX == true)
                    item.SEX_string = "ذكر";
                else
                    item.SEX_string = "انثى";
            }
            if (fname != "")
            {
                Data = Data.Where(x => x.FNAME.Contains(fname)).ToList();
            }
            if (lname != "")
            {
                Data = Data.Where(x => x.LNAME.Contains(lname)).ToList();
            }
            if (natno != "")
            {
                Data = Data.Where(x => x.NATNO == natno).ToList();
            }
            if (father != "")
            {
                Data = Data.Where(x => x.FATHER.Contains(father)).ToList();
            }
            if (mother != "")
            {
                Data = Data.Where(x => x.MOTHER.Contains(mother)).ToList();
            }
            if (bdatey != "")
            {
                Data = Data.Where(x => x.BDATEY.ToString() == bdatey).ToList();
            }
            return Json(Data, JsonRequestBehavior.AllowGet);
        }
        public ActionResult CompanyFilter(string name = "", string comreg_no = "", string comreg_typ = "",
            string city = "", string region = "")
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
            if (name != "")
            {
                Data = Data.Where(x => x.COMPNAME.Contains(name)).ToList();
            }
            if (comreg_no != "")
            {
                Data = Data.Where(x => x.COMREG_NO == comreg_no).ToList();
            }
            if (comreg_typ != "")
            {
                Data = Data.Where(x => x.COMREG_TYP == comreg_typ).ToList();
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
        public ActionResult getPersonByNatno(string natno)
        {
            List<PersonVM> persons = new List<PersonVM>();
            persons = (from a in db.ZPERSON
                       join b in db.ZNATION
                       on a.NAT equals b.NB
                       join c in db.ZPRSTYPE
                       on a.TYP equals c.NB
                       select new PersonVM
                       {
                           NB = a.NB,
                           NATNO = a.NATNO,
                           FNAME = a.FNAME,
                           LNAME = a.LNAME,
                           FATHER = a.FATHER,
                           MOTHER = a.MOTHER,
                           CIVILLOC = a.CIVILLOC,
                           ACTADDRESS = a.ACTADDRESS,
                           ADDRESS = a.ADDRESS,
                           PHONE = a.PHONE,
                           MOBILE = a.MOBILE,
                           BDATED = a.BDATED,
                           BDATEM = a.BDATEM,
                           BDATEY = a.BDATEY,
                           BDATE = a.BDATE,
                           IDCARDNO = a.IDCARDNO,
                           IDCARDDAT = a.IDCARDDAT,
                           BPLACE = a.BPLACE,
                           ALAMANA = a.ALAMANA,
                           SEX = a.SEX,
                           TYP = a.TYP,
                           NAT = a.NAT,
                           NationName = b.NATION,
                           PerType = c.TYPNAME
                       }).Where(x => x.NATNO == natno).ToList();
            var nb = persons.Select(x => x.NB).FirstOrDefault();
            return Json(nb, JsonRequestBehavior.AllowGet);
        }
        public ActionResult getCompanyById(int id)
        {
            List<CompanyVM> companies = new List<CompanyVM>();
            companies = (from a in db.ZCOMPANY
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
                    }).Where(x => x.NB == id).ToList();
            return Json(companies, JsonRequestBehavior.AllowGet);
        }
    }
}
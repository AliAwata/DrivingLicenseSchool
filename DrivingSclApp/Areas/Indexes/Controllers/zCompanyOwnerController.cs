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

namespace DrivingSclApp.Areas.Indexes.Controllers
{
    public class zCompanyOwnerController : Controller
    {
        private DrivingSclEntity db = new DrivingSclEntity();
        public ActionResult Index()
        {
            return View();
        }
        public ActionResult Read([DataSourceRequest] DataSourceRequest request)
        {
            List<CompanyOwnerVM> Data = new List<CompanyOwnerVM>();
            Data = (from co in db.COMPANYOWNER
                    join c in db.ZCOMPANY
                    on co.COMP_NB equals c.NB
                    join p in db.ZPERSON
                    on co.PRS_NB equals p.NB
                    select new CompanyOwnerVM
                    {
                        NB = co.NB,
                        COMP_NB = co.COMP_NB,
                        PRS_NB = co.PRS_NB,
                        JOBTITLE = co.JOBTITLE,
                        NOTE = co.NOTE,
                        COMP_NAME = c.COMPNAME,
                        PRS_NAME = p.FNAME + " " + p.LNAME
                    }).OrderBy(x => x.NB).ToList();
            return Json(Data.ToDataSourceResult(request), JsonRequestBehavior.AllowGet);
        }
        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult Create([DataSourceRequest] DataSourceRequest request, COMPANYOWNER model)
        {
            using (var transaction = db.Database.BeginTransaction())
            {
                if (ModelState.IsValid)
                {
                    try
                    {
                        model.NB = MyDataBase.GetSeqValue("GetIndexID");
                        db.COMPANYOWNER.Add(model);
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
        public ActionResult Update([DataSourceRequest] DataSourceRequest request, COMPANYOWNER model)
        {
            using (var transaction = db.Database.BeginTransaction())
            {
                if (ModelState.IsValid)
                {
                    try
                    {
                        db.COMPANYOWNER.Attach(model);
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
        public ActionResult Destroy([DataSourceRequest] DataSourceRequest request, COMPANYOWNER model)
        {
            using (var transaction = db.Database.BeginTransaction())
            {
                try
                {
                    db.COMPANYOWNER.Attach(model);
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
            return PartialView("_CompanyOwner", Data);
        }
        public ActionResult OwnerByComNb([DataSourceRequest] DataSourceRequest request, int id)
        {
            List<CompanyOwnerVM> Data = new List<CompanyOwnerVM>();
            Data = (from co in db.COMPANYOWNER
                    join c in db.ZCOMPANY
                    on co.COMP_NB equals c.NB
                    join p in db.ZPERSON
                    on co.PRS_NB equals p.NB
                    select new CompanyOwnerVM
                    {
                        NB = co.NB,
                        COMP_NB = co.COMP_NB,
                        PRS_NB = co.PRS_NB,
                        JOBTITLE = co.JOBTITLE,
                        NOTE = co.NOTE,
                        COMP_NAME = c.COMPNAME,
                        PRS_NAME = p.FNAME
                    }).Where(x => x.COMP_NB == id).OrderBy(x => x.NB).ToList();
            ViewData["ID"] = id;
            return Json(Data.ToDataSourceResult(request), JsonRequestBehavior.AllowGet);
        }
        public ActionResult PersonFilter(string fname = "", string lname = "", string natno = "",
            string father = "", string mother = "", string bday = "")
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
            if (bday != "")
            {
                Data = Data.Where(x => x.BDATEY.ToString() == bday).ToList();
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
    }
}
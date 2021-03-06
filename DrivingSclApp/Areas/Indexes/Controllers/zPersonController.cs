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
    public class zPersonController : Controller
    {
        private DrivingSclEntity db = new DrivingSclEntity();
        public ActionResult Index()
        {
            return View();
        }
        public ActionResult Read([DataSourceRequest] DataSourceRequest request)
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
            foreach(var item in Data)
            {
                if (item.SEX == true)
                    item.SEX_string = "ذكر";
                else if(item.SEX == false)
                    item.SEX_string = "انثى";
            }
            return Json(Data.ToDataSourceResult(request), JsonRequestBehavior.AllowGet);
        }
        public ActionResult ReadByFilter(
             string bdatey = "", string natno = "", string fname = "", string lname = "",
            string father = "", string mother = "")
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
                else if (item.SEX == false)
                    item.SEX_string = "انثى";
            }
            if(natno != "")
            {
                Data = Data.Where(x => x.NATNO == natno).ToList();
            } 
            if (fname != "")
            {
                Data = Data.Where(x => x.FNAME.Contains(fname)).ToList();
            }
            if (lname != "")
            {
                Data = Data.Where(x => x.LNAME.Contains(lname)).ToList();
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
        public ActionResult ReadById([DataSourceRequest] DataSourceRequest request, int id)
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
                    }).Where(x => x.NB == id).OrderBy(x => x.NB).ToList();
            foreach (var item in Data)
            {
                if (item.SEX == true)
                    item.SEX_string = "ذكر";
                else
                    item.SEX_string = "انثى";
            }
            return Json(Data.ToDataSourceResult(request), JsonRequestBehavior.AllowGet);
        }
        public ActionResult PrsById(int id)
        {
            var Data = db.ZPERSON.Find(id);
            return Json(Data, JsonRequestBehavior.AllowGet);
        }
        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult Create([DataSourceRequest] DataSourceRequest request, ZPERSON model)
        {
            using (var transaction = db.Database.BeginTransaction())
            {
                if (ModelState.IsValid)
                {
                    try
                    {
                        model.BDATED = model.BDATE.Value.Day;
                        model.BDATEM = model.BDATE.Value.Month;
                        model.BDATEY = model.BDATE.Value.Year;
                        model.NB = Convert.ToInt32(MyDataBase.GetSeqValue("GetIndexID"));
                        db.ZPERSON.Add(model);
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
        public ActionResult Create_getNb([DataSourceRequest] DataSourceRequest request, ZPERSON model)
        {
            using (var transaction = db.Database.BeginTransaction())
            {
                if (ModelState.IsValid)
                {
                    try
                    {
                        model.BDATED = model.BDATE.Value.Day;
                        model.BDATEM = model.BDATE.Value.Month;
                        model.BDATEY = model.BDATE.Value.Year;
                        model.NB = Convert.ToInt32(MyDataBase.GetSeqValue("GetIndexID"));
                        db.ZPERSON.Add(model);
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
        public ActionResult Create_getPrs([DataSourceRequest] DataSourceRequest request, ZPERSON model)
        {
            using (var transaction = db.Database.BeginTransaction())
            {
                if (ModelState.IsValid)
                {
                    try
                    {
                        model.BDATED = model.BDATE.Value.Day;
                        model.BDATEM = model.BDATE.Value.Month;
                        model.BDATEY = model.BDATE.Value.Year;
                        model.NB = MyDataBase.GetSeqValue("GetIndexID");
                        db.ZPERSON.Add(model);
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
        public ActionResult Update([DataSourceRequest] DataSourceRequest request, ZPERSON model)
        {
            using (var transaction = db.Database.BeginTransaction())
            {
                if (ModelState.IsValid)
                {
                    try
                    {
                        model.BDATED = model.BDATE.Value.Day;
                        model.BDATEM = model.BDATE.Value.Month;
                        model.BDATEY = model.BDATE.Value.Year;
                        db.ZPERSON.Attach(model);
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
        public ActionResult Destroy([DataSourceRequest] DataSourceRequest request, ZPERSON model)
        {
            using (var transaction = db.Database.BeginTransaction())
            {
                try
                {
                    db.ZPERSON.Attach(model);
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
        public JsonResult validation(long param1)
        {
            string ss = MyDataBase.RunQuery("select count(*) from ZPERSON where NATNO='" + param1 + "'");
            if (Convert.ToInt32(ss) >= 1)
            {
                return Json(new { success = false, responseText = "الاسم موجود بالفعل" }, JsonRequestBehavior.AllowGet);
            }
            return Json(new { success = true, responseText = "تمت عملية الحفظ بنجاح" }, JsonRequestBehavior.AllowGet);
        }
    }
}
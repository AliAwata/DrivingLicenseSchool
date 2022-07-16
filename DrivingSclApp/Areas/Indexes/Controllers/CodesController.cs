using DrivingSclApp.Models;
using DrivingSclData;
using Kendo.Mvc.Extensions;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace DrivingSclApp.Areas.Indexes.Controllers
{
    public class CodesController : Controller
    {
        DrivingSclEntity db = new DrivingSclEntity();

        public ActionResult GetGovern()
        {
            var res = db.ZGOVERN.Select(x => new {
                Id = x.NB,
                Name = x.NAME
            }).OrderBy(x => x.Id);

            return Json(res, JsonRequestBehavior.AllowGet);
        }

        public ActionResult GetCity()
        {
            var res = db.ZCITY.Select(x => new {
                Id = x.NB,
                Name = x.NAME
            }).OrderBy(x => x.Id);

            return Json(res, JsonRequestBehavior.AllowGet);
        }

        public ActionResult GetCityByGovern(int govern)
        {
            var res = db.ZCITY.Select(x => new {
                Id = x.NB,
                Name = x.NAME,
                Govern = x.GOV_NB
            }).Where(d => d.Govern == govern ).OrderBy(x => x.Id);

            return Json(res, JsonRequestBehavior.AllowGet);
        }

        public ActionResult GetRegion()
        {
            var res = db.ZREGION.Select(x => new {
                Id = x.NB,
                Name = x.NAME
            }).OrderBy(x => x.Id);

            return Json(res, JsonRequestBehavior.AllowGet);
        }

        public ActionResult GetRegionByCity(int city)
        {
            var res = db.ZREGION.Select(x => new {
                Id = x.NB,
                Name = x.NAME,
                City = x.CTY_NB
            }).Where(d => d.City == city).OrderBy(x => x.Id);

            return Json(res, JsonRequestBehavior.AllowGet);
        }

        public ActionResult GetNation()
        {
            var res = db.ZNATION.Select(x => new {
                Id = x.NB,
                Name = x.NATION
            }).OrderBy(x => x.Id);

            return Json(res, JsonRequestBehavior.AllowGet);
        }

        public ActionResult GetPertype()
        {
            var res = db.ZPRSTYPE.Select(x => new {
                Id = x.NB,
                Name = x.TYPNAME
            }).OrderBy(x => x.Id);

            return Json(res, JsonRequestBehavior.AllowGet);
        }

        public ActionResult GetSex()
        {
            List<SelectListItem> sexes = new List<SelectListItem>()
            {
                new SelectListItem() { Text = "ذكر", Value = "true"},
                new SelectListItem() { Text = "أنثى", Value = "false"}
            };
            
            return Json(sexes, JsonRequestBehavior.AllowGet);
        }

        public ActionResult GetComreg_Typ()
        {
            List<SelectListItem> types = new List<SelectListItem>()
            {
                new SelectListItem() { Text = "أفراد", Value = "افراد"},
                new SelectListItem() { Text = "شركات", Value = "شركات"}
            };

            return Json(types, JsonRequestBehavior.AllowGet);
        }

        public ActionResult GetComreg_Typ_list(string typ)
        {
            if (typ == "افراد")
            {
                var persons = db.ZPERSON.Select(x => new
                {
                    Id = x.NB,
                    Name = x.FNAME + x.LNAME,
                    NationNo = x.NATNO
                }).OrderBy(x => x.Id);
                return Json(persons, JsonRequestBehavior.AllowGet);
            }
            else
            {
                var companies = db.ZCOMPANY.Select(x => new
                {
                    Id = x.NB,
                    Name = x.COMPNAME,
                }).OrderBy(x => x.Id);
                return Json(companies, JsonRequestBehavior.AllowGet);
            }
        }

        public ActionResult GetSclStatus()
        {
            var res = db.ZSCLSTATUS.Select(x => new {
                Id = x.NB,
                Name = x.STSNAME
            }).OrderBy(x => x.Id);

            return Json(res, JsonRequestBehavior.AllowGet);
        }

        public ActionResult GetSclType()
        {
            var res = db.ZSCLTYPE.Select(x => new {
                Id = x.NB,
                Name = x.TYPNAME
            }).OrderBy(x => x.Id);

            return Json(res, JsonRequestBehavior.AllowGet);
        }

        public ActionResult GetCategories()
        {
            var res = db.ZCATEGORY.Select(x => new {
                Id = x.NB,
                Name = x.NAME
            }).OrderBy(x => x.Id);
            return Json(res, JsonRequestBehavior.AllowGet);
        }

        public string GetPrev_Category(long id)
        {
            var Data = db.ZCATEGORY.Find(id);

            if (Data.PREV_CATG == null)
            {
                return "";
            }
            else
            {
                return db.ZCATEGORY.Find(Data.PREV_CATG).NAME;
            }
        }

        public ActionResult GetPhoneTyp()
        {
            List<SelectListItem> types = new List<SelectListItem>()
            {
                new SelectListItem() { Text = "أرضي", Value = "1"},
                new SelectListItem() { Text = "محمول", Value = "2"},
                new SelectListItem() { Text = "فاكس", Value = "3"},
                new SelectListItem() { Text = "تلفاكس", Value = "4"}
            };

            return Json(types, JsonRequestBehavior.AllowGet);
        }

        public string GetPhoneTypeName(int id)
        {
            if (id == 1)
                return "أرضي";
            else if (id == 2)
                return "محمول";
            else if (id == 3)
                return "فاكس";
            else if (id == 4)
                return "تلفاكس";
            else
                return "none";
        }
    
        public ActionResult GetVegicleClasses()
        {
            var veh = db.ZVEHICLECLASS.Select(x => new {
                Id = x.NB,
                Name = x.NAME
            }).OrderBy(x => x.Id);
            return Json(veh, JsonRequestBehavior.AllowGet);
        }
    
        public ActionResult GetTrainerType()
        {
            var res = db.ZTRAINERTYPE.Select(x => new
            {
                Id = x.NB,
                Name = x.NAME
            }).OrderBy(x => x.Id);
            return Json(res, JsonRequestBehavior.AllowGet);
        }
    
        public string GetPrsFullName(long prs_nb)
        {
            var prs = db.ZPERSON.Where(x => x.NB == prs_nb).ToList();
            string name;
            name = prs.Select(x => x.FNAME).FirstOrDefault() + " " + prs.Select(x => x.LNAME).FirstOrDefault();
            return name;
        }
    
        public ActionResult GetDocUsage()
        {
            var res = db.ZDOCUSAGE.Select(x => new
            {
                Id = x.NB,
                Name = x.USAGETYPE
            }).OrderBy(x => x.Id);
            return Json(res, JsonRequestBehavior.AllowGet);    
        }
    
        public ActionResult GetDocType()
        {
            var res = db.ZDOCTYPE.Select(x => new
            {
                Id = x.NB,
                Name = x.DOCTYPE
            }).OrderBy(x => x.Id);
            return Json(res, JsonRequestBehavior.AllowGet);
        }
    }
}
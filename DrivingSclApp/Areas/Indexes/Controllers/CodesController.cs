using DrivingSclApp.Models;
using DrivingSclData;
using Kendo.Mvc.Extensions;
using System;
using System.Collections.Generic;
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
            });

            return Json(res, JsonRequestBehavior.AllowGet);
        }

        public ActionResult GetCity()
        {
            var res = db.ZCITY.Select(x => new {
                Id = x.NB,
                Name = x.NAME
            });

            return Json(res, JsonRequestBehavior.AllowGet);
        }

        public ActionResult GetCityByGovern(int govern)
        {
            var res = db.ZCITY.Select(x => new {
                Id = x.NB,
                Name = x.NAME,
                Govern = x.GOV_NB
            }).Where(d => d.Govern == govern );

            return Json(res, JsonRequestBehavior.AllowGet);
        }

        public ActionResult GetRegion()
        {
            var res = db.ZREGION.Select(x => new {
                Id = x.NB,
                Name = x.NAME
            });

            return Json(res, JsonRequestBehavior.AllowGet);
        }

        public ActionResult GetRegionByCity(int city)
        {
            var res = db.ZREGION.Select(x => new {
                Id = x.NB,
                Name = x.NAME,
                City = x.CTY_NB
            }).Where(d => d.City == city);

            return Json(res, JsonRequestBehavior.AllowGet);
        }

        public ActionResult GetNation()
        {
            var res = db.ZNATION.Select(x => new {
                Id = x.NB,
                Name = x.NATION
            });

            return Json(res, JsonRequestBehavior.AllowGet);
        }

        public ActionResult GetPertype()
        {
            var res = db.ZPRSTYPE.Select(x => new {
                Id = x.NB,
                Name = x.TYPNAME
            });

            return Json(res, JsonRequestBehavior.AllowGet);
        }

        public ActionResult GetSex()
        {
            List<SelectListItem> sexes = new List<SelectListItem>()
            {
                new SelectListItem() { Text = "ذكر", Value = "ذكر"},
                new SelectListItem() { Text = "أنثى", Value = "انثى"}
            };
            
            return Json(sexes, JsonRequestBehavior.AllowGet);
        }

        public ActionResult GetComreg_Typ()
        {
            List<SelectListItem> types = new List<SelectListItem>()
            {
                new SelectListItem() { Text = "أفراد", Value = "person"},
                new SelectListItem() { Text = "شركات", Value = "company"}
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
                });
                return Json(persons, JsonRequestBehavior.AllowGet);
            }
            else
            {
                var companies = db.ZCOMPANY.Select(x => new
                {
                    Id = x.NB,
                    Name = x.COMPNAME,
                });
                return Json(companies, JsonRequestBehavior.AllowGet);
            }
        }

        public ActionResult GetSclStatus()
        {
            var res = db.ZSCLSTATUS.Select(x => new {
                Id = x.NB,
                Name = x.STSNAME
            });

            return Json(res, JsonRequestBehavior.AllowGet);
        }

        public ActionResult GetSclType()
        {
            var res = db.ZSCLTYPE.Select(x => new {
                Id = x.NB,
                Name = x.TYPNAME
            });

            return Json(res, JsonRequestBehavior.AllowGet);
        }

        public ActionResult GetCategories()
        {
            var res = db.ZCATEGORY.Select(x => new {
                Id = x.NB,
                Name = x.NAME
            });
            return Json(res, JsonRequestBehavior.AllowGet);
        }
    }
}
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Data.Entity.Infrastructure;
using System.Data.Entity.Validation;
using System.Data.SqlClient;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using DrivingSclApp.Areas.Indexes.Data;
using DrivingSclData;
using Kendo.Mvc.Extensions;
using Kendo.Mvc.UI;

namespace DrivingSclApp.Areas.Indexes.Controllers
{
    [Authorize, Audit, CanDoIt, NoCache, RedirectOnError]
    public class zRegionController : Controller
    {
        private DrivingSclEntity db = new DrivingSclEntity();

        // GET: Indexes/zNation
        public ActionResult Index()
        {
            return View();
        }
        public ActionResult Read([DataSourceRequest] DataSourceRequest request)
        {
            List<RegionVM> Data = new List<RegionVM>();
            Data = (from a in db.ZREGION
                    join b in db.ZCITY
                    on a.CTY_NB equals b.NB
                    select new RegionVM
                    {
                        NB = a.NB,
                        NAME = a.NAME,
                        CTY_NB = a.CTY_NB,
                        CityName = b.NAME
                    }).ToList();
            return Json(Data.ToDataSourceResult(request), JsonRequestBehavior.AllowGet);
        }
        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult Create([DataSourceRequest] DataSourceRequest request, ZREGION model)
        {
            using (var transaction = db.Database.BeginTransaction())
            {
                if (ModelState.IsValid)
                {
                    try
                    {
                        model.NB = MyDataBase.GetSeqValue("GetIndexID");
                        db.ZREGION.Add(model);
                        db.SaveChanges();
                        transaction.Commit();
                    }
                    /*
                    catch (DbUpdateConcurrencyException ex)
                    {
                        transaction.Rollback();
                        return Json(new { success = false, responseText = ex.Message }, JsonRequestBehavior.AllowGet);
                    }
                    catch (DbUpdateException ex)
                    {
                        transaction.Rollback();
                        return Json(new { success = false, responseText = ex.Message }, JsonRequestBehavior.AllowGet);
                    }
                    catch (DbEntityValidationException ex)
                    {
                        transaction.Rollback();
                        return Json(new { success = false, responseText = ex.Message }, JsonRequestBehavior.AllowGet);
                    }
                    catch (SqlException ex)
                    {
                        transaction.Rollback();
                        return Json(new { success = false, responseText = ex.Message }, JsonRequestBehavior.AllowGet);
                    }
                    */
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
        public ActionResult Update([DataSourceRequest] DataSourceRequest request, ZREGION model)
        {
            using (var transaction = db.Database.BeginTransaction())
            {
                if (ModelState.IsValid)
                {
                    try
                    {
                        db.ZREGION.Attach(model);
                        db.Entry(model).State = EntityState.Modified;
                        db.SaveChanges();
                        transaction.Commit();
                    }
                    /*
                    catch (DbUpdateConcurrencyException ex)
                    {
                        transaction.Rollback();
                        return Json(new { success = false, responseText = ex.Message }, JsonRequestBehavior.AllowGet);
                    }
                    catch (DbUpdateException ex)
                    {
                        transaction.Rollback();
                        return Json(new { success = false, responseText = ex.Message }, JsonRequestBehavior.AllowGet);
                    }
                    catch (DbEntityValidationException ex)
                    {
                        transaction.Rollback();
                        return Json(new { success = false, responseText = ex.Message }, JsonRequestBehavior.AllowGet);
                    }
                    catch (SqlException ex)
                    {
                        transaction.Rollback();
                        return Json(new { success = false, responseText = ex.Message }, JsonRequestBehavior.AllowGet);
                    }
                    */
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
        public ActionResult Destroy([DataSourceRequest] DataSourceRequest request, ZREGION model)
        {
            using (var transaction = db.Database.BeginTransaction())
            {
                try
                {
                    db.ZREGION.Attach(model);
                    db.Entry(model).State = EntityState.Deleted;
                    db.SaveChanges();
                    transaction.Commit();
                }
                /*
                catch (DbUpdateConcurrencyException ex)
                {
                    transaction.Rollback();
                    return Json(new { success = false, responseText = ex.InnerException.InnerException.Message }, JsonRequestBehavior.AllowGet);
                }
                catch (DbUpdateException ex)
                {
                    transaction.Rollback();
                    return Json(new { success = false, responseText = ex.InnerException.InnerException.Message }, JsonRequestBehavior.AllowGet);
                }
                catch (DbEntityValidationException ex)
                {
                    transaction.Rollback();
                    return Json(new { success = false, responseText = ex.InnerException.InnerException.Message }, JsonRequestBehavior.AllowGet);
                }
                catch (SqlException ex)
                {
                    transaction.Rollback();
                    return Json(new { success = false, responseText = ex.InnerException.InnerException.Message }, JsonRequestBehavior.AllowGet);
                }
                */
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
            string ss = MyDataBase.RunQuery("select count(*) from ZREGION where Name='" + param1 + "'");
            if (Convert.ToInt32(ss) >= 1)
            {
                return Json(new { success = false, responseText = "الاسم موجود بالفعل" }, JsonRequestBehavior.AllowGet);
            }
            return Json(new { success = true, responseText = "تمت عملية الحفظ بنجاح" }, JsonRequestBehavior.AllowGet);
        }
    }
}
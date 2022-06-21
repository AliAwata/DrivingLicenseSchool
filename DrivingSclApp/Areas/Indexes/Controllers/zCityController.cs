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
    public class zCityController : Controller
    {
        private DrivingSclEntity db = new DrivingSclEntity();
        // GET: Indexes/ZCITY
        public ActionResult Index()
        {            
            return View();
        }
        public ActionResult Read([DataSourceRequest] DataSourceRequest request)
        {
            List<CityVM> Data = new List<CityVM>();
            Data = (from a in db.ZCITY
                    join b in db.ZGOVERN
                    on a.GOV_NB equals b.NB
                    select new CityVM{ 
                        NB = a.NB,
                        NAME = a.NAME,
                        GOV_NB = a.GOV_NB,
                        GOVNAME = b.NAME
                    }).ToList();
            return Json(Data.ToDataSourceResult(request), JsonRequestBehavior.AllowGet);
        }
        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult Create([DataSourceRequest] DataSourceRequest request, ZCITY model)
        {
            using (var transaction = db.Database.BeginTransaction())
            {
                if (ModelState.IsValid)
                {
                    try
                    {
                        model.NB = MyDataBase.GetSeqValue("GetIndexID");
                        db.ZCITY.Add(model);
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
        public ActionResult Update([DataSourceRequest] DataSourceRequest request, ZCITY model)
        {
            using (var transaction = db.Database.BeginTransaction())
            {
                if (ModelState.IsValid)
                {
                    try
                    {
                        db.ZCITY.Attach(model);
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
        public ActionResult Destroy([DataSourceRequest] DataSourceRequest request, ZCITY model)
        {
            using (var transaction = db.Database.BeginTransaction())
            {
                try
                {
                    db.ZCITY.Attach(model);
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
            string ss = MyDataBase.RunQuery("select count(*) from ZCITY where Nation='" + param1 + "'");
            if (Convert.ToInt32(ss) >= 1)
            {
                return Json(new { success = false, responseText = "الاسم موجود بالفعل" }, JsonRequestBehavior.AllowGet);
            }
            return Json(new { success = true, responseText = "تمت عملية الحفظ بنجاح" }, JsonRequestBehavior.AllowGet);
        }
        // GET: Indexes/ZCITY/Details/5
        /*
        public ActionResult Details(long? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            ZCITY ZCITY = db.ZCITY.Find(id);
            if (ZCITY == null)
            {
                return HttpNotFound();
            }
            return View(ZCITY);
        }

        // GET: Indexes/ZCITY/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: Indexes/ZCITY/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "NB,NATION,CNTRYNAME")] ZCITY ZCITY)
        {
            if (ModelState.IsValid)
            {
                db.ZCITY.Add(ZCITY);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            return View(ZCITY);
        }

        // GET: Indexes/ZCITY/Edit/5
        public ActionResult Edit(long? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            ZCITY ZCITY = db.ZCITY.Find(id);
            if (ZCITY == null)
            {
                return HttpNotFound();
            }
            return View(ZCITY);
        }

        // POST: Indexes/ZCITY/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "NB,NATION,CNTRYNAME")] ZCITY ZCITY)
        {
            if (ModelState.IsValid)
            {
                db.Entry(ZCITY).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            return View(ZCITY);
        }

        // GET: Indexes/ZCITY/Delete/5
        public ActionResult Delete(long? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            ZCITY ZCITY = db.ZCITY.Find(id);
            if (ZCITY == null)
            {
                return HttpNotFound();
            }
            return View(ZCITY);
        }

        // POST: Indexes/ZCITY/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(long id)
        {
            ZCITY ZCITY = db.ZCITY.Find(id);
            db.ZCITY.Remove(ZCITY);
            db.SaveChanges();
            return RedirectToAction("Index");
        }
        
        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }
        */
    }
}

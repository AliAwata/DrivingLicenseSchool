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
    public class zNationController : Controller
    {
        private DrivingSclEntity db = new DrivingSclEntity();

        // GET: Indexes/zNation
        public ActionResult Index()
        {
            return View();
        }
        public ActionResult Read([DataSourceRequest] DataSourceRequest request)
        {
            List<ZNATION> Data = db.ZNATION.ToList();
            return Json(Data.ToDataSourceResult(request));
        }
        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult Create([DataSourceRequest] DataSourceRequest request, ZNATION model)
        {
            using (var transaction = db.Database.BeginTransaction())
            {
                if (ModelState.IsValid)
                {
                    try
                    {
                        model.NB = MyDataBase.GetSeqValue("GetIndexID");
                        db.ZNATION.Add(model);
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
        public ActionResult Update([DataSourceRequest] DataSourceRequest request, ZNATION model)
        {
            using (var transaction = db.Database.BeginTransaction())
            {
                if (ModelState.IsValid)
                {
                    try
                    {
                        db.ZNATION.Attach(model);
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
        public ActionResult Destroy([DataSourceRequest] DataSourceRequest request, ZNATION model)
        {
            using (var transaction = db.Database.BeginTransaction())
            {
                try
                {
                    db.ZNATION.Attach(model);
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
            string ss = MyDataBase.RunQuery("select count(*) from zNation where Nation='" + param1 + "'");
            if (Convert.ToInt32(ss) >= 1)
            {
                return Json(new { success = false, responseText = "الاسم موجود بالفعل" }, JsonRequestBehavior.AllowGet);
            }
            return Json(new { success = true, responseText = "تمت عملية الحفظ بنجاح" }, JsonRequestBehavior.AllowGet);
        }
        // GET: Indexes/zNation/Details/5
        /*
        public ActionResult Details(long? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            ZNATION zNATION = db.ZNATION.Find(id);
            if (zNATION == null)
            {
                return HttpNotFound();
            }
            return View(zNATION);
        }

        // GET: Indexes/zNation/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: Indexes/zNation/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "NB,NATION,CNTRYNAME")] ZNATION zNATION)
        {
            if (ModelState.IsValid)
            {
                db.ZNATION.Add(zNATION);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            return View(zNATION);
        }

        // GET: Indexes/zNation/Edit/5
        public ActionResult Edit(long? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            ZNATION zNATION = db.ZNATION.Find(id);
            if (zNATION == null)
            {
                return HttpNotFound();
            }
            return View(zNATION);
        }

        // POST: Indexes/zNation/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "NB,NATION,CNTRYNAME")] ZNATION zNATION)
        {
            if (ModelState.IsValid)
            {
                db.Entry(zNATION).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            return View(zNATION);
        }

        // GET: Indexes/zNation/Delete/5
        public ActionResult Delete(long? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            ZNATION zNATION = db.ZNATION.Find(id);
            if (zNATION == null)
            {
                return HttpNotFound();
            }
            return View(zNATION);
        }

        // POST: Indexes/zNation/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(long id)
        {
            ZNATION zNATION = db.ZNATION.Find(id);
            db.ZNATION.Remove(zNATION);
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

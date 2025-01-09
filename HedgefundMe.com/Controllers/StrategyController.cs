using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using HedgefundMe.com.Models;

namespace HedgefundMe.com.Controllers
{
     [Authorize]
    public class StrategyController : Controller
    {
        private ProjectEntities db = new ProjectEntities();

        //
        // GET: /Strategy/

        public ActionResult Index()
        {
            return View(db.StockPickingStrategies.ToList());
        }

        //
        // GET: /Strategy/Details/5

        public ActionResult Details(int id = 0)
        {
            StockPickingStrategy stockpickingstrategy = db.StockPickingStrategies.Find(id);
            if (stockpickingstrategy == null)
            {
                return HttpNotFound();
            }
            return View(stockpickingstrategy);
        }

        //
        // GET: /Strategy/Create

        public ActionResult Create()
        {
            return View();
        }

        //
        // POST: /Strategy/Create

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(StockPickingStrategy stockpickingstrategy)
        {
            if (ModelState.IsValid)
            { 
                db.StockPickingStrategies.Add(stockpickingstrategy);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            return View(stockpickingstrategy);
        }

        //
        // GET: /Strategy/Edit/5

        public ActionResult Edit(int id = 0)
        {
            StockPickingStrategy stockpickingstrategy = db.StockPickingStrategies.Find(id);
            if (stockpickingstrategy == null)
            {
                return HttpNotFound();
            }
            return View(stockpickingstrategy);
        }

        //
        // POST: /Strategy/Edit/5

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(StockPickingStrategy stockpickingstrategy)
        {
            if (ModelState.IsValid)
            {
                db.Entry(stockpickingstrategy).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            return View(stockpickingstrategy);
        }

        //
        // GET: /Strategy/Delete/5

        public ActionResult Delete(int id = 0)
        {
            StockPickingStrategy stockpickingstrategy = db.StockPickingStrategies.Find(id);
            if (stockpickingstrategy == null)
            {
                return HttpNotFound();
            }
            return View(stockpickingstrategy);
        }

        //
        // POST: /Strategy/Delete/5

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            StockPickingStrategy stockpickingstrategy = db.StockPickingStrategies.Find(id);
            db.StockPickingStrategies.Remove(stockpickingstrategy);
            db.SaveChanges();
            return RedirectToAction("Index");
        }

        protected override void Dispose(bool disposing)
        {
            db.Dispose();
            base.Dispose(disposing);
        }
    }
}
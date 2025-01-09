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
    public class PicksController : Controller
    {
        private ProjectEntities db = new ProjectEntities();

        //
        // GET: /Picks/

        public ActionResult Index(int id)
        {
            //try to find all the picks with this id
            var picks = db.StockPicks.Where(f => f.Strategy.StockPickingStrategyID == id).ToList();
            ViewBag.StrategyId = id;
            return View(picks);
        }

        //
        // GET: /Picks/Details/5

        public ActionResult Details(int id = 0)
        {
            StockPick stockpick = db.StockPicks.Find(id);
            if (stockpick == null)
            {
                return HttpNotFound();
            }
            return View(stockpick);
        }

        //
        // GET: /Picks/Create

        public ActionResult Create(int id )
        {
            StockPick s = new StockPick();
            s.Strategy = db.StockPickingStrategies.Find(id);
            
            return View(s);
        }

        //
        // POST: /Picks/Create

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(StockPick stockpick)
        {
            if (ModelState.IsValid)
            {
                db.StockPicks.Add(stockpick);
                db.SaveChanges();
                return RedirectToAction("Index", new { id = stockpick.Strategy.StockPickingStrategyID });
            }

            return View(stockpick);
        }

        //
        // GET: /Picks/Edit/5

        public ActionResult Edit(int id = 0)
        {
            StockPick stockpick = db.StockPicks.Find(id);
            if (stockpick == null)
            {
                return HttpNotFound();
            }
            return View(stockpick);
        }

        //
        // POST: /Picks/Edit/5

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(StockPick stockpick)
        {
            if (ModelState.IsValid)
            {
                db.Entry(stockpick).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index", new { id = stockpick.Strategy.StockPickingStrategyID });
            }
            return View(stockpick);
        }

        //
        // GET: /Picks/Delete/5

        public ActionResult Delete(int id = 0)
        {
            StockPick stockpick = db.StockPicks.Find(id);
            if (stockpick == null)
            {
                return HttpNotFound();
            }
            return View(stockpick);
        }

        //
        // POST: /Picks/Delete/5

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            StockPick stockpick = db.StockPicks.Find(id);
            db.StockPicks.Remove(stockpick);
            db.SaveChanges();
            return RedirectToAction("Index", new { id = stockpick.Strategy.StockPickingStrategyID });
        }

        protected override void Dispose(bool disposing)
        {
            db.Dispose();
            base.Dispose(disposing);
        }
    }
}
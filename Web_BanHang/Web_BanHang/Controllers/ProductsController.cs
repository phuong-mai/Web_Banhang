﻿using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using Web_BanHang.Models;
using EntityState = System.Data.EntityState;

namespace Web_BanHang.Controllers
{
    public class ProductsController : Controller
    {
        private BanHangEntities db = new BanHangEntities();

        // GET: Products
        public ActionResult Index()
        {
            var products = db.Products.Include(p => p.Catalog).Include(p => p.ProductDetail).Include(p => p.Storage);
            return View(products.ToList());
        }

        // GET: Products/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Product product = db.Products.Find(id);
            if (product == null)
            {
                return HttpNotFound();
            }
            return View(product);
        }

        // GET: Products/Create
        public ActionResult Create()
        {
            ViewBag.Catalog_ID = new SelectList(db.Catalogs, "ID", "Catalog_Name");
            ViewBag.ID = new SelectList(db.ProductDetails, "Product_ID", "Product_Detail");
            ViewBag.ID = new SelectList(db.Storages, "Product_ID", "Product_ID");
            return View();
        }

        // POST: Products/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "ID,Product_Name,Catalog_ID,Amount,Price,Image_Name")] Product product)
        {
            if (ModelState.IsValid)
            {
                db.Products.Add(product);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            ViewBag.Catalog_ID = new SelectList(db.Catalogs, "ID", "Catalog_Name", product.Catalog_ID);
            ViewBag.ID = new SelectList(db.ProductDetails, "Product_ID", "Product_Detail", product.ID);
            ViewBag.ID = new SelectList(db.Storages, "Product_ID", "Product_ID", product.ID);
            return View(product);
        }

        // GET: Products/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Product product = db.Products.Find(id);
            if (product == null)
            {
                return HttpNotFound();
            }
            ViewBag.Catalog_ID = new SelectList(db.Catalogs, "ID", "Catalog_Name", product.Catalog_ID);
            ViewBag.ID = new SelectList(db.ProductDetails, "Product_ID", "Product_Detail", product.ID);
            ViewBag.ID = new SelectList(db.Storages, "Product_ID", "Product_ID", product.ID);
            return View(product);
        }

        // POST: Products/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "ID,Product_Name,Catalog_ID,Amount,Price,Image_Name")] Product product)
        {
            if (ModelState.IsValid)
            {
                db.Entry(product).State = (System.Data.Entity.EntityState)EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            ViewBag.Catalog_ID = new SelectList(db.Catalogs, "ID", "Catalog_Name", product.Catalog_ID);
            ViewBag.ID = new SelectList(db.ProductDetails, "Product_ID", "Product_Detail", product.ID);
            ViewBag.ID = new SelectList(db.Storages, "Product_ID", "Product_ID", product.ID);
            return View(product);
        }

        // GET: Products/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Product product = db.Products.Find(id);
            if (product == null)
            {
                return HttpNotFound();
            }
            return View(product);
        }

        // POST: Products/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            Product product = db.Products.Find(id);
            db.Products.Remove(product);
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
    }
}

using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using Web_BanHang.Models;

namespace Web_BanHang.Controllers
{
    public class HomeController : Controller
    {
        private BanHangEntities db = new BanHangEntities();
        public ActionResult Index()
        {
            var products = db.Products.Include(p => p.Catalog).Include(p => p.ProductDetail).Include(p => p.Storage);
            return View(products.ToList());
        }

        public ActionResult About()
        {
            ViewBag.Message = "Your application description page.";

            return View();
        }

        public ActionResult Contact()
        {
            ViewBag.Message = "Your contact page.";

            return View();
        }
    }
}
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

        public ActionResult Cart()
        {
            return View();
        }

        public ActionResult Checkout()
        {
            return View();
        }

        public ActionResult Detail()
        {
            return View();
        }

        public ActionResult Login()
        {
            return View();
        }

        public ActionResult Register()
        {
            return View();
        }
    }
}
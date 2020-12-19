using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Security.Cryptography;
using System.Text;
using System.Web;
using System.Web.Mvc;
using Web_BanHang.Models;
using PagedList;

namespace Web_BanHang.Controllers
{
    public class HomeController : Controller
    {
        //public class MultipleModelInOneView
        //{
        //    public IEnumerable<Web_BanHang.Models.Product> lstMaster { get; set; }
        //    public IEnumerable<Web_BanHang.Models.> lstOtherRecords { get; set; }

        //    public MultipleModelInOneView()
        //    {
        //        lstMaster = new public List<webapplication1.Models.table1>();
        //        lstOtherRecords = new public List<webapplication1.Models.table2>();
        //    }
        //}
    
        private BanHangEntities db = new BanHangEntities();
        public ActionResult Index(int? page)
        {
            if (page == null) page = 1;
            var products = db.Products.OrderBy(x => x.ID).Include(p => p.Catalog).Include(p => p.ProductDetail).Include(p => p.Storage);
            int pageSize = 8;
            int pageNumber = (page ?? 1);           
            return View(products.ToPagedList(pageNumber, pageSize));
        }

        public ActionResult Cart()
        {
            return View();
        }

        public ActionResult Checkout()
        {
            return View();
        }

        public ActionResult Detail(int? id)
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

        public ActionResult Laptop()
        {
            var products = db.Products.Where(p => p.Catalog_ID == 1).Include(p => p.Catalog).Include(p => p.ProductDetail).Include(p => p.Storage);
            return View(products.ToList());
        }

        public ActionResult Phone()
        {
            var products = db.Products.Where(p => p.Catalog_ID == 2).Include(p => p.Catalog).Include(p => p.ProductDetail).Include(p => p.Storage);
            return View(products.ToList());
        }

        public ActionResult Accessories()
        {
            var products = db.Products.Where(p => p.Catalog_ID == 3).Include(p => p.Catalog).Include(p => p.ProductDetail).Include(p => p.Storage);
            return View(products.ToList());
        }

        public ActionResult Combo()
        {
            return View(db.Comboes.ToList());
        }

        public ActionResult DetailCombo(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Combo combo = db.Comboes.Find(id);
            if (combo == null)
            {
                return HttpNotFound();
            }
            return View(combo);
        }

        // GET: Register
        public ActionResult Register()
        {
            return View();
        }

        // POST: Register
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Register(Customer customer)
        {
            if (ModelState.IsValid)
            {
                //var check = db.Customers.FirstOrDefault(s => s.phoneNumber == customer.phoneNumber);
                //if (check == null)
                //{
                    customer.password = GetMD5(customer.password);
                    db.Customers.Add(customer);
                    db.SaveChanges();
                    return RedirectToAction("Index  ");
                //}
                //else
                //{
                //    ViewBag.error = "Số điện thoại đã tồn tại";
                //    return View();
                //}

            }

            return View(customer);
        }

        public static string GetMD5(string str)
        {
            MD5 md5 = new MD5CryptoServiceProvider();
            byte[] fromData = Encoding.UTF8.GetBytes(str);
            byte[] targetData = md5.ComputeHash(fromData);
            string byte2String = null;

            for (int i = 0; i < targetData.Length; i++)
            {
                byte2String += targetData[i].ToString("x2");

            }
            return byte2String;
        }

        public ActionResult Login()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Login(string phoneNumber, string password)
        {
            if (ModelState.IsValid)
            {
                var f_password = GetMD5(password);
                var data = db.Customers.Where(s => s.phoneNumber.Equals(phoneNumber) && s.password.Equals(f_password)).ToList();
                if (data.Count() > 0)
                {
                    //add session
                    Session["FullName"] = data.FirstOrDefault().firstName + " " + data.FirstOrDefault().lastName;
                    Session["phoneNumber"] = data.FirstOrDefault().phoneNumber;
                    Session["ID"] = data.FirstOrDefault().ID;
                    return RedirectToAction("Index");
                }
                else
                {
                    ViewBag.error = "Login failed";
                    return RedirectToAction("Login");
                }
            }
            return View();
        }


        //Logout
        public ActionResult Logout()
        {
            Session.Clear();//remove session
            return RedirectToAction("Login");
        }
         [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Buy(int id)
        {
            var products = db.Products.Where(s => s.ID.Equals(id)).ToList();

            if (Session["cart"] == null)
            {
                List<Item> cart = new List<Item>();
                cart.Add(new Item { Product = products , Quantity = 1 });
                Session["cart"] = cart;
            }
            else
            {
                List<Item> cart = (List<Item>)Session["cart"];
                int index = isExist(id);
                if (index != -1)
                {
                    cart[index].Quantity++;
                }
                else
                {
                    cart.Add(new Item { Product = products, Quantity = 1 });
                }
                Session["cart"] = cart;
            }
            return RedirectToAction("Cart");

        }
        public ActionResult Remove(int id)
        {
            List<Item> cart = (List<Item>)Session["cart"];
            int index = isExist(id);
            cart.RemoveAt(index);
            Session["cart"] = cart;
            return RedirectToAction("Cart");
        }

        private int isExist(int id)
        {
            List<Item> cart = (List<Item>)Session["cart"];
            for (int i = 0; i < cart.Count; i++)
                if (cart[i].Product[0].ID.Equals(id))
                    return i;
            return -1;
        }

    }
}
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
        public ActionResult BuyCombo(int id)
        {
            var combos = db.Comboes.Where(s => s.ID.Equals(id)).ToList();

            if (Session["combo"] == null)
            {
                if (Session["product"] == null)
                {
                    List<Product> product2 = new List<Product>();
                    Session["product"] = product2;
                }
                List<ItemCombo> combo = new List<ItemCombo>();
                List<Product> product = (List<Product>)Session["product"];
                var accepted = true;
                var abc = combos[0].Product_List;
                List<string> results = abc.Split(';').ToList();

                    
                foreach (var result in results)
                {
                    var products = db.Products.Find(Int32.Parse(result));
                    product.Add(products);

                    if (product.Find(x => x.ID == Int32.Parse(result)) != null)
                    {
                        var amount = product.Find(x => x.ID == Int32.Parse(result)).Amount;
                        if (amount <= 0)
                        {
                            accepted = false;
                            alert alert = new alert { Alert = "", Name = product.Find(x => x.ID == Int32.Parse(result)).Product_Name };
                            Session["alert"] = alert;
                            //combo[0].Stock = 0;
                            return RedirectToAction("Cart");
                        }
                        else
                        {
                            product.Find(x => x.ID == Int32.Parse(result)).Amount--;
                            Session["product"] = product;
                        }
                    }
                    else
                    {
                        var amount = product.Find(x => x.ID == Int32.Parse(result)).Amount;
                        if (amount <= 0)
                        {
                            accepted = false;
                            alert alert = new alert { Alert = "", Name = product.Find(x => x.ID == Int32.Parse(result)).Product_Name };
                            Session["alert"] = alert;
                            //combo[0].Stock = 0;
                            return RedirectToAction("Cart");
                        }
                        else
                        {
                            product.Add(products);
                            product.Find(x => x.ID == Int32.Parse(result)).Amount--;
                            Session["product"] = product;
                        }    
                        
                    }    
                }

                if (accepted)
                {
                    combo.Add(new ItemCombo { Combo = combos, Quantity = 1, Stock = 1 });
                    Session["combo"] = combo;
                }

                

            }
            else
            {
                if (Session["product"] == null)
                {
                    List<Product> product2 = new List<Product>();
                    Session["product"] = product2;
                }
                List<ItemCombo> combo = (List<ItemCombo>)Session["combo"];
                List<Product> product = (List<Product>)Session["product"];
                int index = isExist2(id);
                if (index != -1)
                {
                    var abc = combo[index].Combo[0].Product_List;
                    List<string> results = abc.Split(';').ToList();
                    var accepted = true;
                    foreach( var result in results)
                    {
                        var products = db.Products.Find(Int32.Parse(result));
                        if (product.Find(x => x.ID == Int32.Parse(result)) != null)
                        {
                            var amount = product.Find(x => x.ID == Int32.Parse(result)).Amount;
                            if (amount <= 0)
                            {
                                alert alert = new alert { Alert = "", Name = product.Find(x => x.ID == Int32.Parse(result)).Product_Name };
                                Session["alert"] = alert;
                                accepted = false;
                                //combo[index].Stock = 0;
                                return RedirectToAction("Cart");
                            }
                            else
                            {
                                product.Find(x => x.ID == Int32.Parse(result)).Amount--;
                                Session["product"] = product;
                            }
                        }
                        else
                        {
                            product.Add(products);
                            var amount = product.Find(x => x.ID == Int32.Parse(result)).Amount;
                            if (amount <= 0)
                            {
                                alert alert = new alert { Alert = "", Name = product.Find(x => x.ID == Int32.Parse(result)).Product_Name };
                                Session["alert"] = alert;
                                accepted = false;
                                //combo[index].Stock = 0;
                                return RedirectToAction("Cart");
                            }
                            else
                            {
                                product.Find(x => x.ID == Int32.Parse(result)).Amount--;
                                Session["product"] = product;
                            }
                        }    
                       

                    }

                    if (accepted)
                    {
                        combo[index].Quantity++;
                        Session["combo"] = combo;
                        //cart.Add(new Item { Product = products, Quantity = 1 });
                        //Session["cart"] = cart;
                        //return RedirectToAction("Cart");
                        //db.Products.Find(id).Amount = db.Products.Find(id).Amount - 1;
                        //db.SaveChanges();
                    }
                }
                else
                {
                    var abc = combos[0].Product_List;
                    List<string> results = abc.Split(';').ToList();
                    var accepted = true;
                    foreach (var result in results)
                    {
                        var products = db.Products.Find(Int32.Parse(result));
                        if (product.Find(x => x.ID == Int32.Parse(result)) != null)
                        {
                            var amount = product.Find(x => x.ID == Int32.Parse(result)).Amount;
                            if (amount <= 0)
                            {
                                alert alert = new alert { Alert = "", Name = product.Find(x => x.ID == Int32.Parse(result)).Product_Name };
                                Session["alert"] = alert;
                                accepted = false;
                                //combo[0].Stock = 0;
                                return RedirectToAction("Cart");
                            }
                            else
                            {
                                product.Find(x => x.ID == Int32.Parse(result)).Amount--;
                                Session["product"] = product;
                            }
                        }
                        else
                        {
                            product.Add(products);
                            product.Find(x => x.ID == Int32.Parse(result)).Amount--;
                            Session["product"] = product;
                        }
                    }
                    if (accepted)
                        {   
                            combo.Add(new ItemCombo { Combo = combos, Quantity = 1, Stock = 1 });
                            Session["combo"] = combo;
                        }    
                       
                }
               
            }
            return RedirectToAction("Cart");

        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Buy(int id)
        {
            var products = db.Products.Where(s => s.ID.Equals(id)).ToList();
            if(Session["product"] == null)
            {
                if (Session["cart"] == null)
                {
                    List<Product> product = new List<Product>();
                    List<Item> cart = new List<Item>();
                    cart.Add(new Item { Product = products, Quantity = 1, Stock = 1 });
                    product.Add(products[0]);
                    product.Find(x => x.ID == id).Amount--;
                    Session["cart"] = cart;
                    Session["product"] = product;
                }
                else
                {
                    List<Item> cart = (List<Item>)Session["cart"];
                    List<Product> product = (List<Product>)Session["product"];

                    int index = isExist(id);
                    if (index != -1)
                    {
                        if (product.Find(x => x.ID == id).Amount <= 0)
                        {
                            //cart[index].Stock = 0;
                            alert alert = new alert { Alert = "", Name = product.Find(x => x.ID == id).Product_Name };
                            Session["alert"] = alert;
                            Session["cart"] = cart;
                            return RedirectToAction("Cart");
                        }
                        else
                        {
                            cart[index].Quantity++;
                            product.Find(x => x.ID == id).Amount--;
                            Session["product"] = product;
                            //cart.Add(new Item { Product = products, Quantity = 1 });
                            //Session["cart"] = cart;
                            //return RedirectToAction("Cart");
                            //db.Products.Find(id).Amount = db.Products.Find(id).Amount - 1;
                            //db.SaveChanges();
                        }

                    }
                    else
                    {
                        product.Add(products[0]);
                        product.Find(x => x.ID == id).Amount--;
                        Session["product"] = product;
                        cart.Add(new Item { Product = products, Quantity = 1, Stock = 1 });
                    }
                    Session["cart"] = cart;
                }
            }
            else
            {
                if (Session["cart"] == null)
                {
                    List<Product> product = (List<Product>)Session["product"];
                    if (product.Find(x => x.ID == id).Amount > 0)
                    {
                        List<Item> cart = new List<Item>();
                        cart.Add(new Item { Product = products, Quantity = 1, Stock = 1 });
                        product.Find(x => x.ID == id).Amount--;
                        Session["product"] = product;
                        Session["cart"] = cart;
                    }
                    else
                    {
                        alert alert = new alert { Alert = "", Name = product.Find(x => x.ID == id).Product_Name };
                        Session["alert"] = alert;
                        return RedirectToAction("Cart");
                    }    

                }
                else
                {
                    List<Item> cart = (List<Item>)Session["cart"];
                    List<Product> product2 = (List<Product>)Session["product"];
                    if(product2.Find(x => x.ID == id) == null)
                    {
                        product2.Add(products[0]);
                    }
                    Session["product"] = product2;

                    List<Product> product = (List<Product>)Session["product"];
                    if (product.Find(x => x.ID == id).Amount > 0)
                    {
                        int index = isExist(id);
                        if (index != -1)
                        {
                            if (product.Find(x => x.ID == id).Amount <= 0)
                            {
                                //cart[index].Stock = 0;
                                alert alert = new alert { Alert = "", Name = product.Find(x => x.ID == id).Product_Name };
                                Session["alert"] = alert;
                                Session["cart"] = cart;
                                return RedirectToAction("Cart");
                            }
                            else
                            {
                                cart[index].Quantity++;
                                product.Find(x => x.ID == id).Amount--;
                                Session["product"] = product;
                                //cart.Add(new Item { Product = products, Quantity = 1 });
                                //Session["cart"] = cart;
                                //return RedirectToAction("Cart");
                                //db.Products.Find(id).Amount = db.Products.Find(id).Amount - 1;
                                //db.SaveChanges();
                            }

                        }
                        else
                        {
                            product.Add(products[0]);
                            product.Find(x => x.ID == id).Amount--;
                            Session["product"] = product;
                            cart.Add(new Item { Product = products, Quantity = 1, Stock = 1 });
                        }
                        Session["cart"] = cart;
                    }
                    else
                    {
                        alert alert = new alert { Alert = "", Name = product.Find(x => x.ID == id).Product_Name };
                        Session["alert"] = alert;
                        return RedirectToAction("Cart");
                    }

                }
            }    

           
            return RedirectToAction("Cart");

        }
        public ActionResult Remove(int id)
        {
            Session.Remove("alert");
            List<Item> cart = (List<Item>)Session["cart"];
            int index = isExist(id);
            List<Product> product = (List<Product>)Session["product"];
            product.Find(x => x.ID == id).Amount = product.Find(x => x.ID == id).Amount + cart[index].Quantity;
            cart.RemoveAt(index);
            Session["cart"] = cart;
            Session["product"] = product;
            return RedirectToAction("Cart");
        }

        public ActionResult Remove2(int id)
        {
            Session.Remove("alert");
            List<ItemCombo> combos = (List<ItemCombo>)Session["combo"];
            int index = isExist2(id);
            List<Product> product = (List<Product>)Session["product"];

            var abc = db.Comboes.Find(id).Product_List;
            List<string> results = abc.Split(';').ToList();

            foreach (var result in results)
            {
                product.Find(x => x.ID == Int32.Parse(result)).Amount = product.Find(x => x.ID == Int32.Parse(result)).Amount + combos[index].Quantity;
            }
            Session["product"] = product;
            combos.RemoveAt(index);
            Session["combo"] = combos;
            
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
        private int isExist2(int id)
        {
            List<ItemCombo> combo = (List<ItemCombo>)Session["combo"];
            for (int i = 0; i < combo.Count; i++)
                if (combo[i].Combo[0].ID.Equals(id))
                    return i;
            return -1;
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult AddToInvoice(Invoice invoice)
        {
            var total = 0;
            if(Session["cart"] != null && Session["combo"] != null)
            {
                List<Item> cart = (List<Item>)Session["cart"];
                var total1 = cart.Sum(item => item.Product[0].Price * item.Quantity);
                List<ItemCombo> combo = (List<ItemCombo>)Session["combo"];
                var total2 = combo.Sum(item => item.Combo[0].totalMoney * item.Quantity);
                total = (Int32)total1 + (Int32)total2;
            }
            if (Session["cart"] != null )
            {
                List<Item> cart = (List<Item>)Session["cart"];
                var total1 = cart.Sum(item => item.Product[0].Price * item.Quantity);
                total = (Int32)total1;
            }
            if (Session["combo"] != null)
            {
                List<ItemCombo> combo = (List<ItemCombo>)Session["combo"];
                var total2 = combo.Sum(item => item.Combo[0].totalMoney * item.Quantity);
                total = (Int32)total2;
            }

            var newID = db.Invoices.OrderByDescending(p => p.ID)
                            .Select(r => r.ID)
                            .First();


            db.Invoices.Add(new Invoice
            {
                ID = newID + 1,
                Customer_ID = (Int32)Session["ID"],
                totalMoney = total,
                customerAddress = invoice.customerAddress

            }) ;
            db.SaveChanges();

            if(Session["cart"] != null)
            {
                foreach (var item in (List<Item>)Session["cart"])
                {
                    db.InvoiceDetails.Add(new InvoiceDetail { Product_ID = item.Product[0].ID, Invoice_ID = newID + 1, Amount = item.Quantity, Price = item.Product[0].Price * item.Quantity });
                    db.Products.Find(item.Product[0].ID).Amount = db.Products.Find(item.Product[0].ID).Amount - item.Quantity;
                    db.SaveChanges();
                    Session["cart"] = null;
                }
            }
            
            if(Session["combo"] != null)
            {
                foreach (var item in (List<ItemCombo>)Session["combo"])
                {
                    db.InvoiceDetails.Add(new InvoiceDetail { Combo_ID = item.Combo[0].ID, Invoice_ID = newID + 1, Amount = item.Quantity, Price = item.Combo[0].totalMoney * item.Quantity });
                    var abc = db.Comboes.Find(item.Combo[0].ID).Product_List;
                    List<string> results = abc.Split(';').ToList();

                    foreach (var result in results)
                    {
                        db.Products.Find(Int32.Parse(result)).Amount = db.Products.Find(Int32.Parse(result)).Amount - item.Quantity;
                    }
                    db.SaveChanges();
                    Session["combo"] = null;
                }
            }    

            

            

            return RedirectToAction("Index");
        }

    }
}
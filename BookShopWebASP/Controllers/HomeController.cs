using BookShopWebASP.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace BookShopWebASP.Controllers
{
    public class HomeController : Controller
    {
        public static int PageLength = 20;

        // Show all products
        public ActionResult Index()
        {
            return View();
        }

        public JsonResult IndexBook(int Page)
        {
            return Json(DataProvider.Instance.DataBase.Products.Select(x => new
            {
                Id = x.Id,
                IdImageData = x.ImageData.Id,
                DisplayName = x.DisplayName,
                DisplayImage = x.ImageData.DisplayName,
                Price = x.Price,
                Discount = x.Discount,
            }).ToList().Skip((Page - 1) * PageLength).Take(PageLength), JsonRequestBehavior.AllowGet);
        }

        public JsonResult FavouriteBook(int Page)
        {
            return Json(DataProvider.Instance.DataBase.Products.OrderByDescending(x => x.OutputInfoes.Sum(y => y.Count)).Select(x => new
            {
                Id = x.Id,
                IdImageData = x.ImageData.Id,
                DisplayName = x.DisplayName,
                DisplayImage = x.ImageData.DisplayName,
                Price = x.Price,
                Discount = x.Discount,
            }).ToList().Skip((Page - 1) * PageLength).Take(PageLength), JsonRequestBehavior.AllowGet);
        }

        public JsonResult CategoryBook(string CategoryName, int Page)
        {
            return Json(DataProvider.Instance.DataBase.Products.Where(x => x.Category.DisplayName == CategoryName).Select(x => new
            {
                Id = x.Id,
                IdImageData = x.ImageData.Id,
                DisplayName = x.DisplayName,
                DisplayImage = x.ImageData.DisplayName,
                Price = x.Price,
                Discount = x.Discount,
            }).ToList().Skip((Page - 1) * PageLength).Take(PageLength), JsonRequestBehavior.AllowGet);
        }

        // Show product with productid
        public ActionResult ShowProduct(string ProductID)
        {
            var book = DataProvider.Instance.DataBase.Products.Where(x => x.Id == ProductID).SingleOrDefault();
            if (book == null) return RedirectToAction("Index", "Home");
            return View(book);
        }

        // Buy product with productid
        public ActionResult Buy(string ProductID, int Count, bool FromBasket)
        {
            // If haven't login => login and come back
            if (Session["UserID"] == null)
                return RedirectToAction("Login", "Users", new { @returnUrl = Request.Url.OriginalString });

            string userid = Session["UserID"].ToString();
            User user = DataProvider.Instance.DataBase.Users.First(x => x.Id == userid);
            Product product = DataProvider.Instance.DataBase.Products.Where(x => x.Id == ProductID).SingleOrDefault();

            if (product == null) return RedirectToAction("Index", "Home");
            BuyModel model = new BuyModel()
            {
                ProductID = ProductID,
                Count = Count,
                FullName = user.FullName,
                ProductDisplayName = product.DisplayName,
                ProductPrice = product.Price.Value,
                FromBasket = FromBasket,
            };
            ViewBag.Bought = null;
            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Buy(BuyModel model)
        {
            // If haven't login => login and come back
            if (Session["UserID"] == null)
                return RedirectToAction("Login", "Users", new { @returnUrl = Request.Url.OriginalString });

            if (ModelState.IsValid)
            {
                string userid = Session["UserID"].ToString();
                string hashPass = DataProvider.getHashPass(model.Password);
                User user = DataProvider.Instance.DataBase.Users.Where(x => x.Id == userid && x.Password == hashPass).SingleOrDefault();
                if (user != null)
                {
                    // Add new output
                    var output = DataProvider.Instance.DataBase.Outputs.Where(x => x.IdCustomer == userid).SingleOrDefault();
                    if (output == null)
                    {
                        output = new Output() { Id = Guid.NewGuid().ToString(), IdCustomer = userid };
                        DataProvider.Instance.DataBase.Outputs.Add(output);
                        DataProvider.Instance.DataBase.SaveChanges();
                    }
                    var product = DataProvider.Instance.DataBase.Products.Where(x => x.Id == model.ProductID).SingleOrDefault();
                    OutputInfo outputinfo = new OutputInfo()
                    {
                        Id = Guid.NewGuid().ToString(),
                        IdProduct = model.ProductID,
                        IdOutput = output.Id,
                        OutputPrice = product.Price.Value * (100 - product.Discount.Value) / 100,
                        Count = model.Count,
                        DateOutput = DateTime.Now,
                        IsCheckOut = 0,
                    };
                    DataProvider.Instance.DataBase.OutputInfoes.Add(outputinfo);
                    // remove from basket
                    if (model.FromBasket)
                    {
                        var basket = DataProvider.Instance.DataBase.Baskets.Where(x => x.IdCustomer == userid).SingleOrDefault();
                        if (basket != null)
                        {
                            var bi = DataProvider.Instance.DataBase.BasketInfoes.Where(x => x.IdProduct == model.ProductID && x.IdBasket == basket.Id).SingleOrDefault();
                            if (bi != null)
                            {
                                DataProvider.Instance.DataBase.BasketInfoes.Remove(bi);
                                if (basket.BasketInfoes.Count == 0)
                                    DataProvider.Instance.DataBase.Baskets.Remove(basket);
                            }
                        }
                    }
                    DataProvider.Instance.DataBase.SaveChanges();

                    ViewBag.Bought = true;
                    return View(model);
                }
                else
                {
                    ModelState.AddModelError("", "Your password is incorrect!");
                }
            }

            ViewBag.Bought = null;
            return View(model);
        }

        // Add to basket product with productid
        public ActionResult AddToBasket(string ProductID)
        {
            // If haven't login => login and come back
            if (Session["UserID"] == null)
                return RedirectToAction("Login", "Users", new { @returnUrl = Request.Url.OriginalString });
            string userid = Session["UserID"].ToString();
            var basket = DataProvider.Instance.DataBase.Baskets.Where(x => x.IdCustomer == userid).SingleOrDefault();
            if (basket == null)
            {
                basket = new Basket()
                {
                    Id = Guid.NewGuid().ToString(),
                    IdCustomer = userid,
                };
                DataProvider.Instance.DataBase.Baskets.Add(basket);
                DataProvider.Instance.DataBase.SaveChanges();
            }
            var bi = DataProvider.Instance.DataBase.BasketInfoes.Where(x => x.IdProduct == ProductID && x.IdBasket == basket.Id).SingleOrDefault();
            if (bi == null)
            {
                bi = new BasketInfo()
                {
                    Id = Guid.NewGuid().ToString(),
                    IdBasket = basket.Id,
                    IdProduct = ProductID,
                    Count = 1,
                };
                DataProvider.Instance.DataBase.BasketInfoes.Add(bi);
            }
            else
            {
                bi.Count++;
            }
            DataProvider.Instance.DataBase.SaveChanges();

            return RedirectToAction("ShowProduct", "Home", new { @ProductID = ProductID });
        }

        public ActionResult RemoveFromBasket(string ProductID)
        {
            // If haven't login => login and come back
            if (Session["UserID"] == null)
                return RedirectToAction("Index", "Home");
            string userid = Session["UserID"].ToString();
            var basket = DataProvider.Instance.DataBase.Baskets.Where(x => x.IdCustomer == userid).SingleOrDefault();
            if (basket == null)
                return RedirectToAction("Index", "Home");
            var bi = DataProvider.Instance.DataBase.BasketInfoes.Where(x => x.IdProduct == ProductID && x.IdBasket == basket.Id).SingleOrDefault();
            if (bi == null)
                return RedirectToAction("Index", "Home");
            else
            {
                DataProvider.Instance.DataBase.BasketInfoes.Remove(bi);
                if (basket.BasketInfoes.Count == 0)
                    DataProvider.Instance.DataBase.Baskets.Remove(basket);
                DataProvider.Instance.DataBase.SaveChanges();
            }

            return RedirectToAction("Basket", "Home");
        }

        // History
        public ActionResult History()
        {
            if (Session["UserID"] == null) return RedirectToAction("Index", "Home");
            string userid = Session["UserID"].ToString();
            var model = DataProvider.Instance.DataBase.Outputs.Where(x => x.IdCustomer == userid).SingleOrDefault();

            return View(model);
        }

        public ActionResult Basket()
        {
            if (Session["UserID"] == null) return RedirectToAction("Index", "Home");
            string userid = Session["UserID"].ToString();
            var model = DataProvider.Instance.DataBase.Baskets.Where(x => x.IdCustomer == userid).SingleOrDefault();

            return View(model);
        }

        public ActionResult Favourite()
        {
            return View();
        }

        public ActionResult Category(string CategoryName)
        {
            var model = DataProvider.Instance.DataBase.Products.Where(x => x.Category.DisplayName == CategoryName);
            ViewBag.Title = CategoryName;
            return View(model);
        }
    }
}
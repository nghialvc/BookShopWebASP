using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace BookShopWebASP.Controllers
{
    public class SupplierController : Controller
    {
        public static int PageLength = 5;
        //
        // GET: /Supplier/
        public ActionResult Index()
        {
            if (Session["UserAuthority"] == null || (Session["UserAuthority"].ToString() != "Supplier")) return RedirectToAction("Index", "Home");
            int Page = 1;
            var topPage = (from row in DataProvider.Instance.DataBase.InputInfoes select row.Id).Take(PageLength * (Page - 1));
            var model = DataProvider.Instance.DataBase.InputInfoes.Where(x =>!topPage.Contains(x.Id)).Take(PageLength).ToList();
            int countmodel = DataProvider.Instance.DataBase.InputInfoes.Count();
            @ViewBag.MaxPage = countmodel / PageLength + (countmodel % PageLength != 0 ? 1 : 0);
            @ViewBag.Page = Page;
            return View("Index", new SupplierData() { Option = "Input", Data = model });
        }

        public ActionResult ShowInput(int Page)
        {
            if (Session["UserAuthority"] == null || (Session["UserAuthority"].ToString() != "Supplier")) return RedirectToAction("Index", "Home");
            var topPage = (from row in DataProvider.Instance.DataBase.InputInfoes select row.Id).Take(PageLength * (Page - 1));
            var model = DataProvider.Instance.DataBase.InputInfoes.Where(x => !topPage.Contains(x.Id)).Take(PageLength).ToList();
            int countmodel = DataProvider.Instance.DataBase.InputInfoes.Count();
            @ViewBag.MaxPage = countmodel / PageLength + (countmodel % PageLength != 0 ? 1 : 0);
            @ViewBag.Page = Page;
            return View("Index", new SupplierData() { Option = "Input", Data = model });
        }

        public ActionResult ShowProduct(int Page)
        {
            if (Session["UserAuthority"] == null || (Session["UserAuthority"].ToString() != "Supplier")) return RedirectToAction("Index", "Home");
            var userid = Session["UserID"].ToString();
            var topPage = (from database in DataProvider.Instance.DataBase.InputInfoes where database.IdSupplier == userid select database.Input.Product.Id).Distinct().Take(PageLength * (Page - 1)).ToList();
            var model = DataProvider.Instance.DataBase.Products.Where(x => !topPage.Contains(x.Id)).Take(PageLength).ToList();
            int countmodel = DataProvider.Instance.DataBase.Products.Count();
            @ViewBag.MaxPage = countmodel / PageLength + (countmodel % PageLength != 0 ? 1 : 0);
            @ViewBag.Page = Page;
            return View("Index", new SupplierData() { Option = "Product", Data = model });
        }

        // Support
        public ActionResult ViewProduct(string ProductID)
        {
            var model = DataProvider.Instance.DataBase.Products.Where(x => x.Id == ProductID).SingleOrDefault();
            return View("Product", model);
        }
    }
    public class SupplierData
    {
        public string Option { get; set; }

        public object Data { get; set; }
    }
}
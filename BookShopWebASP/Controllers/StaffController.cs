using BookShopWebASP.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace BookShopWebASP.Controllers
{
    public class StaffController : Controller
    {
        public static int PageLength = 5;
        //
        // GET: /Staff/
        public ActionResult Index()
        {
            if (Session["UserAuthority"] == null || (Session["UserAuthority"].ToString() != "Staff" && Session["UserAuthority"].ToString() != "Admin")) return RedirectToAction("Index", "Home");
            int Page = 1;
            var topPage = (from row in DataProvider.Instance.DataBase.OutputInfoes select row.Id).Take(PageLength * (Page - 1));
            var model = DataProvider.Instance.DataBase.OutputInfoes.Where(x => x.IsCheckOut == 0 && !topPage.Contains(x.Id)).Take(PageLength).ToList();
            int countmodel = DataProvider.Instance.DataBase.OutputInfoes.Count(x => x.IsCheckOut == 0);
            @ViewBag.MaxPage = countmodel / PageLength + (countmodel % PageLength != 0 ? 1 : 0);
            @ViewBag.Page = Page;
            return View("Index", new StaffData() { Option = "UnCheckedOut", Data = model });
        }
        public ActionResult ShowCheckedOut(int Page)
        {
            if (Session["UserAuthority"] == null || (Session["UserAuthority"].ToString() != "Staff" && Session["UserAuthority"].ToString() != "Admin")) return RedirectToAction("Index", "Home");
            var topPage = (from row in DataProvider.Instance.DataBase.OutputInfoes select row.Id).Take(PageLength * (Page - 1));
            var model = DataProvider.Instance.DataBase.OutputInfoes.Where(x => x.IsCheckOut == 1 && !topPage.Contains(x.Id)).Take(PageLength).ToList();
            int countmodel = DataProvider.Instance.DataBase.OutputInfoes.Count(x => x.IsCheckOut == 1);
            @ViewBag.MaxPage = countmodel / PageLength + (countmodel % PageLength != 0 ? 1 : 0);
            @ViewBag.Page = Page;
            return View("Index", new StaffData() { Option = "CheckedOut", Data = model });
        }
        public ActionResult ShowUnCheckedOut(int Page)
        {
            if (Session["UserAuthority"] == null || (Session["UserAuthority"].ToString() != "Staff" && Session["UserAuthority"].ToString() != "Admin")) return RedirectToAction("Index", "Home");
            var topPage = (from row in DataProvider.Instance.DataBase.OutputInfoes select row.Id).Take(PageLength * (Page - 1));
            var model = DataProvider.Instance.DataBase.OutputInfoes.Where(x => x.IsCheckOut == 0 && !topPage.Contains(x.Id)).Take(PageLength).ToList();
            int countmodel = DataProvider.Instance.DataBase.OutputInfoes.Count(x => x.IsCheckOut == 0);
            @ViewBag.MaxPage = countmodel / PageLength + (countmodel % PageLength != 0 ? 1 : 0);
            @ViewBag.Page = Page;
            return View("Index", new StaffData() { Option = "UnCheckedOut", Data = model });
        }
        public ActionResult ShowProduct(int Page)
        {
            if (Session["UserAuthority"] == null || (Session["UserAuthority"].ToString() != "Staff" && Session["UserAuthority"].ToString() != "Admin")) return RedirectToAction("Index", "Home");
            var topPage = (from row in DataProvider.Instance.DataBase.Products select row.Id).Take(PageLength * (Page - 1));
            var model = DataProvider.Instance.DataBase.Products.Where(x => !topPage.Contains(x.Id)).Take(PageLength).ToList();
            int countmodel = DataProvider.Instance.DataBase.Products.Count();
            @ViewBag.MaxPage = countmodel / PageLength + (countmodel % PageLength != 0 ? 1 : 0);
            @ViewBag.Page = Page;
            return View("Index", new StaffData() { Option = "Product", Data = model });
        }
        public ActionResult ShowCustomer(int Page)
        {
            if (Session["UserAuthority"] == null || (Session["UserAuthority"].ToString() != "Staff" && Session["UserAuthority"].ToString() != "Admin")) return RedirectToAction("Index", "Home");
            var topPage = (from row in DataProvider.Instance.DataBase.Users select row.Id).Take(PageLength * (Page - 1));
            var model = DataProvider.Instance.DataBase.Users.Where(x =>!topPage.Contains(x.Id)).Take(PageLength).ToList();
            int countmodel = DataProvider.Instance.DataBase.Users.Count();
            @ViewBag.MaxPage = countmodel / PageLength + (countmodel % PageLength != 0 ? 1 : 0);
            @ViewBag.Page = Page;
            return View("Index", new StaffData() { Option = "Customer", Data = model });
        }

        // Support View
        public ActionResult CheckOut(string OutputInfoID, string ReturnUrl)
        {
            if (Session["UserAuthority"] == null || (Session["UserAuthority"].ToString() != "Staff" && Session["UserAuthority"].ToString() != "Admin")) return RedirectToAction("Index", "Home");
            var outputinfo = DataProvider.Instance.DataBase.OutputInfoes.Where(x => x.Id == OutputInfoID).SingleOrDefault();
            if (outputinfo != null)
            {
                outputinfo.DateCheckOut = DateTime.Now;
                outputinfo.IsCheckOut = 1;
                DataProvider.Instance.DataBase.SaveChanges();
            }
            return Redirect(ReturnUrl);
        }
        public ActionResult Remove(string OutputInfoID, string ReturnUrl)
        {
            if (Session["UserAuthority"] == null || (Session["UserAuthority"].ToString() != "Staff" && Session["UserAuthority"].ToString() != "Admin")) return RedirectToAction("Index", "Home");
            var outputinfo = DataProvider.Instance.DataBase.OutputInfoes.Where(x => x.Id == OutputInfoID).SingleOrDefault();
            if (outputinfo != null)
            {
                DataProvider.Instance.DataBase.OutputInfoes.Remove(outputinfo);
                DataProvider.Instance.DataBase.SaveChanges();
            }
            return Redirect(ReturnUrl);
        }

        public ActionResult ViewUser(string UserID)
        {
            var model = DataProvider.Instance.DataBase.Users.Where(x => x.Id == UserID).SingleOrDefault();
            return View("User", model);
        }
    }

    public class StaffData
    {
        public string Option { get; set; }

        public object Data { get; set; }
    }
}
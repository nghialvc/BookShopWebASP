using BookShopWebASP.Models;
using Microsoft.Owin.Security;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;

namespace BookShopWebASP.Controllers
{
    public class UsersController : Controller
    {
        // Login
        [AllowAnonymous]
        public ActionResult Login(string returnUrl)
        {
            if (Session["UserID"] != null) return RedirectToAction("Index", "Home");
            ViewBag.returnUrl = returnUrl;
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Login(string returnUrl, User model)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    var hashPass = DataProvider.getHashPass(model.Password);
                    var user = DataProvider.Instance.DataBase.Users.Where(x => x.UserName == model.UserName && x.Password == hashPass).SingleOrDefault();
                    if (user != null)
                    {
                        Session["UserID"] = user.Id;
                        Session["UserName"] = user.UserName;
                        Session["UserAuthority"] = user.UserAuthority.DisplayName;
                        return Redirect(returnUrl);
                    }
                    else
                    {
                        ModelState.AddModelError("", "Invalid username or password.");
                    }
                }
                catch
                {
                    ModelState.AddModelError("", "Invalid username or password.");
                }
            }

            // If we got this far, something failed, redisplay form
            ViewBag.returnUrl = returnUrl;
            return View();
        }

        // Register New Account
        [AllowAnonymous]
        public ActionResult Register()
        {
            if (Session["UserID"] != null) return RedirectToAction("Index", "Home");
            return View();
        }

        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public ActionResult Register(RegisterModel model)
        {
            if (ModelState.IsValid)
            {
                if (DataProvider.Instance.DataBase.Users.Where(x => x.UserName == model.UserName).SingleOrDefault() != null)
                {
                    ModelState["UserName"].Errors.Add("The UserName is existed. Please enter other UserName and try again!");
                }
                else
                {
                    string hashPass = DataProvider.getHashPass(model.Password);
                    User user = new User()
                    {
                        Id = Guid.NewGuid().ToString(),
                        UserName = model.UserName,
                        Password = hashPass,
                        FullName = model.FullName,
                        Address = model.Address,
                        Email = model.Email,
                        Phone = model.Phone,
                        Birthday = model.Birthday,
                        Point = 0,
                        IdUserAuthority = DataProvider.Instance.DataBase.UserAuthorities.Where(x => x.DisplayName == "Customer").SingleOrDefault().Id,
                    };
                    DataProvider.Instance.DataBase.Users.Add(user);
                    DataProvider.Instance.DataBase.SaveChanges();
                    Session["UserID"] = user.Id;
                    Session["UserName"] = user.UserName;
                    Session["UserAuthority"] = user.UserAuthority.DisplayName;
                    return RedirectToAction("Index", "Home");
                }
            }

            // If we got this far, something failed, redisplay form
            return View();
        }

        // Manage Account
        public ActionResult Manage()
        {
            if (Session["UserID"] == null) return RedirectToAction("Index", "Home");
            string id = Session["UserID"].ToString();
            User user = DataProvider.Instance.DataBase.Users.Where(x => x.Id == id).SingleOrDefault();
            ManageModel model = new ManageModel()
            {
                FullName = user.FullName,
                Address = user.Address,
                Email = user.Email,
                Phone = user.Phone,
                Birthday = user.Birthday.Value.ToString("MM/dd/yyyy"),
            };
            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Manage(ManageModel model)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    DateTime? birthday = DateTime.Parse(model.Birthday);
                    string hashPass = DataProvider.getHashPass(model.Password);
                    string username = Session["UserName"].ToString();
                    User user = DataProvider.Instance.DataBase.Users.Where(x => x.UserName == username && x.Password == hashPass).SingleOrDefault();
                    if (user != null)
                    {
                        user.FullName = model.FullName;
                        user.Address = model.Address;
                        user.Email = model.Email;
                        user.Phone = model.Phone;
                        user.Birthday = birthday;

                        DataProvider.Instance.DataBase.SaveChanges();
                        ModelState.AddModelError("", "Your information was updated!");
                    }
                    else
                    {
                        ModelState["Password"].Errors.Add("Your password is incorrect! Please try again");
                    }
                }
                catch
                {
                    ModelState.AddModelError("", "Your birthday format is invalid! Please try again");
                }
            }

            // If we got this far, something failed, redisplay form
            return View(model);
        }

        // Change Password
        public ActionResult ChangePassword()
        {
            if (Session["UserID"] == null) return RedirectToAction("Index", "Home");
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult ChangePassword(ChangePasswordModel model)
        {
            if (ModelState.IsValid)
            {
                string hashPass = DataProvider.getHashPass(model.Password);
                string hashNewPass = DataProvider.getHashPass(model.NewPassword);
                string username = Session["UserName"].ToString();
                User user = DataProvider.Instance.DataBase.Users.Where(x => x.UserName == username && x.Password == hashPass).SingleOrDefault();
                if (user != null)
                {
                    user.Password = hashNewPass;
                    DataProvider.Instance.DataBase.SaveChanges();
                    ModelState.AddModelError("", "Your password was changed!");
                }
                else
                {
                    ModelState["Password"].Errors.Add("Your password is incorrect! Please try again");
                }
            }

            // If we got this far, something failed, redisplay form
            return View();
        }

        public ActionResult Logout()
        {
            Session["UserID"] = null;
            Session["UserName"] = null;
            Session["UserAuthority"] = null;
            return RedirectToAction("Index", "Home");
        }
    }
}
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Data.Linq.Mapping;
using System.Data;
using BookShopWebASP.Models;

namespace BookShopWebASP.Controllers
{
    public class AdminController : Controller
    {
        public static int PageLength = 5;

        public static Dictionary<string, List<string>> Tables = new Dictionary<string, List<string>>()
        {
            {"UserAuthority",new List<string>() { "Id", "DisplayName" }},
            {"Users",new List<string>() { "Id", "UserName", "FullName", "Email", "Phone", "Address", "Birthday", "Point", "IdUserAuthority" }},
            {"Category",new List<string>() { "Id", "DisplayName" }},
            {"Unit",new List<string>() { "Id", "DisplayName" }},
            {"Product",new List<string>() { "Id", "IdUnit", "IdCategory", "IdImageData", "DisplayName", "Price", "Discount" }},
            {"InputInfo",new List<string>() { "Id", "IdInput", "IdSupplier","DateInput", "Count", "InputPrice"}},
            {"OutputInfo",new List<string>() { "Id", "DateOutput", "Count", "IdProduct", "IsCheckOut", "DateCheckOut"  }},
            {"BasketInfo",new List<string>() { "Id", "IdBasket", "IdProduct", "Count" }},
        };

        public ActionResult Index()
        {
            return RedirectToAction("ViewTable", "Admin", new { Page = 1, TableName = Tables.Keys.First() });
        }

        public ActionResult ViewTable(int Page, string TableName, string ModelId)
        {
            if (Session["UserAuthority"] == null || Session["UserAuthority"].ToString() != "Admin") return RedirectToAction("Index", "Home");
            DataTable model = new DataTable();
            string query = "Select ";
            foreach (var item in Tables[TableName])
            {
                query += item + " ,";
            }
            query = query.Remove(query.Length - 1) + "from dbo." + TableName;
            if (string.IsNullOrEmpty(ModelId))
                model = DataProvider.Instance.ExecuteQuery(query + " order by Id offset " + ((Page - 1) * PageLength).ToString() + " rows fetch next " + PageLength.ToString() + " rows only;");
            else
                model = DataProvider.Instance.ExecuteQuery(query + " Where Id = @Id", new object[] { ModelId });

            int count = (int)DataProvider.Instance.ExecuteQuery("Select count(*) as count from dbo." + TableName).Rows[0]["count"];

            ViewBag.TableName = TableName;
            ViewBag.Page = Page;
            ViewBag.MaxPage = count / PageLength + (count % PageLength == 0 ? 0 : 1);
            if (ModelId != null)
            {
                ViewBag.ModelId = ModelId;
            }
            return View("Index", model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult ViewTable(int Page, string TableName, string ModelId, AdminModel model)
        {
            if (Session["UserAuthority"] == null || Session["UserAuthority"].ToString() != "Admin") return RedirectToAction("Index", "Home");

            if (model.Method == "Add")
            {
                AddData(TableName, ModelId, model);
            }
            else if (model.Method == "Update")
            {
                UpdateData(TableName, ModelId, model);
            }
            else if (model.Method == "Delete")
            {
                DeleteData(TableName, ModelId);
                Page = 1;
                ModelId = null;
            }
            return RedirectToAction("ViewTable", "Admin", new { Page = Page, TableName = TableName, ModelId = ModelId });
        }

        public ActionResult ImageData(int Page, string ModelID)
        {
            ViewBag.ModelId = ModelID;
            List<ImageData> data = new List<ImageData>();
            if (ModelID != null)
                data = DataProvider.Instance.DataBase.ImageDatas.Where(x => x.Id == ModelID).ToList();
            else
                data = DataProvider.Instance.DataBase.ImageDatas.ToList();
            ImageModel model = new ImageModel()
            {
                FileAttach = null,
                Images = data,
            };
            int count = DataProvider.Instance.DataBase.ImageDatas.Count();
            ViewBag.Page = Page;
            ViewBag.MaxPage = count / PageLength + (count % PageLength == 0 ? 0 : 1);
            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult ImageData(int Page, string ModelID, ImageModel model)
        {
            ViewBag.ModelId = ModelID;
            switch (model.Method)
            {
                case "Add":
                    if (ModelState.IsValid)
                    {
                        AddImage(model.FileAttach);
                    }
                    break;
                case "Update":
                    if (ModelState.IsValid)
                    {
                        UpdateImage(ModelID, model.FileAttach);
                    }
                    break;
                case "Delete":
                    DeleteData("ImageData", ModelID);
                    Page = 1;
                    ModelID = null;
                    break;
            }
            return RedirectToAction("ImageData", "Admin", new { Page = Page, ModelID = ModelID });
        }

        // Support
        private void AddData(string TableName, string ModelId, AdminModel model)
        {
            string query = "Insert Into dbo." + TableName + "(";
            foreach (var item in Tables[TableName])
            {
                query += item + " ,";
            }
            query = query.Remove(query.Length - 1) + ") Values (";
            foreach (var item in Tables[TableName])
            {
                if (item == "Id")
                {
                    model.Id = Guid.NewGuid().ToString();
                }
                object data = model.GetType().GetProperty(item).GetValue(model);
                if (data != null)
                    if (data.GetType().Name == "String")
                        query += "N'" + data.ToString() + "' ,";
                    else if (data.GetType().Name == "DateTime")
                        query += "'" + data.ToString() + "' ,";
                    else
                        query += data.ToString() + " ,";
                else
                    query += " null,";
            }
            query = query.Remove(query.Length - 1) + ")";

            DataProvider.Instance.ExecuteQuery(query);
            DataProvider.Instance.DataBase.SaveChanges();
        }

        private void UpdateData(string TableName, string ModelId, AdminModel model)
        {
            string query = "Update dbo." + TableName + " Set ";
            foreach (var item in Tables[TableName])
            {
                if (item == "Id") continue;
                object data = model.GetType().GetProperty(item).GetValue(model);
                if (data != null)
                    if (data.GetType().Name == "String")
                        query += item + " = N'" + data.ToString() + "' ,";
                    else if (data.GetType().Name == "DateTime")
                        query += item + " = '" + data.ToString() + "' ,";
                    else
                        query += item + " = " + data.ToString() + " ,";
                else
                    query += item + " = null ,";
            }
            query = query.Remove(query.Length - 1) + " Where Id = N'" + ModelId + "'";

            DataProvider.Instance.ExecuteQuery(query);
            DataProvider.Instance.DataBase.SaveChanges();
        }

        private void DeleteData(string TableName, string ModelId)
        {
            string query = "Delete dbo." + TableName + " Where Id=N'" + ModelId + "'";
            DataProvider.Instance.ExecuteQuery(query);
            DataProvider.Instance.DataBase.SaveChanges();
        }

        private void AddImage(HttpPostedFileBase model)
        {
            byte[] uploadedFile = new byte[model.InputStream.Length];
            model.InputStream.Read(uploadedFile, 0, uploadedFile.Length);

            // Saving info.  
            DataProvider.Instance.DataBase.ImageDatas.Add(new ImageData() { Id = Guid.NewGuid().ToString(), DisplayName = model.FileName, Extension = model.ContentType, BaseData = Convert.ToBase64String(uploadedFile) });
            DataProvider.Instance.DataBase.SaveChanges();
        }

        private void UpdateImage(string ModelId, HttpPostedFileBase model)
        {
            byte[] uploadedFile = new byte[model.InputStream.Length];
            model.InputStream.Read(uploadedFile, 0, uploadedFile.Length);

            var data = DataProvider.Instance.DataBase.ImageDatas.Where(x => x.Id == ModelId).SingleOrDefault();
            if (data != null)
            {
                // Saving info.  
                data.DisplayName = model.FileName;
                data.Extension = model.ContentType;
                data.BaseData = Convert.ToBase64String(uploadedFile);
                DataProvider.Instance.DataBase.SaveChanges();
            }
        }
    }
}
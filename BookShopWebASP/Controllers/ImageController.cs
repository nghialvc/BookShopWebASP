using BookShopWebASP.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace BookShopWebASP.Controllers
{
    public class ImageController : Controller
    {
        //
        // GET: /Image/
        public ActionResult Index(string ImageID)
        {
            // Info.  
            return RedirectToAction("Index", "Home");
        }

        public ActionResult ShowImage(string ImageID)
        {
            try
            {// Loading dile info.  
                ImageData fileInfo = DataProvider.Instance.DataBase.ImageDatas.Where(x => x.Id == ImageID).SingleOrDefault();
                if (fileInfo == null)
                    return null;
                // Info.  
                return GetFile(fileInfo.BaseData, fileInfo.Extension);
            }
            catch
            {
                return ShowImage(ImageID);
            }
        }

        private FileResult GetFile(string fileContent, string fileContentType)
        {
            // Initialization.  
            FileResult file = null;

            try
            {
                // Get file.  
                byte[] byteContent = Convert.FromBase64String(fileContent);
                file = this.File(byteContent, fileContentType);
            }
            catch (Exception ex)
            {
                // Info.  
                throw ex;
            }

            // info.  
            return file;
        }
    }
}
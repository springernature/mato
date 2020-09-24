using MATO.DataModel;
using MATO.NET.Persistence;
using MATO.NET.ViewModels;
using MATO.NET.ViewModels.v2Models;
using MATO.Classes;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http.Headers;
using System.Reflection;
using System.Web;
using System.Web.Mvc;

namespace MATO.NET.Controllers
{
    public class AuthorController : Controller
    {
        private readonly IUnitOfWork _unitOfWork;

        public AuthorController()
        {
            _unitOfWork = new UnitOfWork(new MATOContext());
        }

        // GET: Calendar
        public ActionResult Index(string id)
        {
            if (id == null)
            {
                AuthorViewModel model =
                    new AuthorViewModel { Authors = _unitOfWork.UserService.GetApplicationUsersInRole("Author") };
                return View(model);
            }
            else
            {
                IndividualAuthorViewModel model = new IndividualAuthorViewModel();
                model.Author = _unitOfWork.UserService.GetUserById(id);
                model.Calendar = _unitOfWork.UserService.GetUserCalendarByUser(model.Author);
                model.Files = _unitOfWork.RequestsService.GetAuthorFiles(model.Author);
                model.AuthorDetail = _unitOfWork.UserService.GetAuthorDetails(id);

                return View("Individual", model);

            }
        }

        public JsonResult GetAuthorFiles(string id)
        {
            var author = _unitOfWork.RequestsService.GetAuthorDetail(id);
            var files = _unitOfWork.RequestsService.GetAuthorFiles(author);

            var json = JsonConvert.SerializeObject(new { files }, new Newtonsoft.Json.Converters.StringEnumConverter());
            return Json(json);
        }

        [HttpPost]
        public ActionResult UploadFile(IEnumerable<HttpPostedFileBase> files, string authorId)
        {
            Uri url = Request.Url;
            string id = HttpUtility.ParseQueryString(url.Query).Get("authorId");
            var author = _unitOfWork.UserService.GetUserById(authorId);

            foreach (var file in files)
            {
                if (file.ContentLength > 0)
                {
                    var authorName = author.FullName.Replace(" ", "");
                    var fileName = ContentDispositionHeaderValue.Parse(file.FileName.Trim('"').Replace(" ", "_"));
                    var webRoot = System.Web.HttpContext.Current.Request.Url.Host;
                    var localPath = AppDomain.CurrentDomain.BaseDirectory;
                    var folder = localPath + "Resources\\" + authorName;
                    var host = Request.UserHostName;
                    var path = "//" + webRoot + "/Resources/" + authorName + "/" + fileName;
                    var pathToSave = localPath + "Resources\\" + authorName + "\\" + fileName;

                    if (!Directory.Exists(folder))
                    {
                        Directory.CreateDirectory(folder);
                    }

                    file.SaveAs(pathToSave);

                    var res = _unitOfWork.AdminService.CreateResourceEntry(path, author);
                }
            }

            // Do whatever, this is an AJAX based call, the return just ends the post call.
            var json = JsonConvert.SerializeObject("", new Newtonsoft.Json.Converters.StringEnumConverter());
            return Json(json);
        }

        public JsonResult RemoveAuthorFile(int id)
        {
            AuthorFiles f = _unitOfWork.AdminService.GetAuthorFileById(id);
            
            if (f != null)
            {
                _unitOfWork.AdminService.RemoveAuthorFile(f);
                return Json("success"); ;
            }
            else
            {
                return Json("error"); ;
            }
        }

        [HttpPost]
        public ActionResult Edit( IndividualAuthorViewModel model, HttpPostedFileBase file)
        {
            Uri url = Request.Url;
            var author = _unitOfWork.UserService.GetUserById(model.Author.Id);

            author.FirstName = model.Author.FirstName;
            author.LastName = model.Author.LastName;
            author.FullName = model.Author.FirstName + " " + model.Author.LastName;
            author.Email = model.Author.Email;

            var aud = _unitOfWork.UserService.GetAuthorDetails(model.Author.Id);
            aud.AuthorNotes = model.AuthorDetail.AuthorNotes;

            //if(file.ContentLength >= 1)
            if(file != null)
            {
                if (file.ContentLength > 0)
                {
                    //var fileName = ContentDispositionHeaderValue.Parse(file.FileName.Trim('"'));
                    var fileName = Path.GetFileName(file.FileName.Trim());
                    var webRoot = System.Web.HttpContext.Current.Request.Url.Host;
                    var localPath = AppDomain.CurrentDomain.BaseDirectory;
                    var folder = localPath + "\\Content\\AuthorPictures\\" + model.Author.FirstName + "_" + model.Author.LastName;
                    var host = Request.UserHostName;
                    var path = "//" + webRoot + "/Content/AuthorPictures/" + model.Author.FirstName + "_" + model.Author.LastName + "/" + fileName;
                    var pathToSave = localPath + "Content\\AuthorPictures\\" + model.Author.FirstName + "_" + model.Author.LastName + "\\" + fileName;

                    if (!Directory.Exists(folder))
                    {
                        Directory.CreateDirectory(folder);
                    }

                    file.SaveAs(pathToSave);

                    aud.AuthorPicture = "/Content/AuthorPictures/" + model.Author.FirstName + "_" + model.Author.LastName + "/" + fileName;
                }
            }

            _unitOfWork.Complete();

            return RedirectToAction("Index", new { id = author.Id } );
        }
    }
}
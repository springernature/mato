using MATO.DataModel;
using MATO.NET.Persistence;
using MATO.NET.ViewModels;
using Newtonsoft.Json;
using System;
using System.Web.Mvc;

namespace MATO.NET.Controllers
{
    public class TitleController : Controller
    {
        private readonly IUnitOfWork _unitOfWork;

        public TitleController()
        {
            _unitOfWork = new UnitOfWork(new MATOContext());
        }
        // GET: Title
        public ActionResult Index()

        {
            return View();   
        }
        public ActionResult GetAllTitleWithTagetSector()
        {
            var titleWithTargetSectorList = _unitOfWork.RequestsService.GetPromotedTitleWithTargetSectors();
            var json = JsonConvert.SerializeObject(new { titleWithTargetSectorList }, new Newtonsoft.Json.Converters.StringEnumConverter());
            return Json(json, JsonRequestBehavior.AllowGet);
        }

        public JsonResult GetAuthorsWithPaymentType(string id)
        {
            var title = Int32.Parse(id);
            var authorWithPaymentTypeList = _unitOfWork.AdminService.GetAuthorsAndPaymenetType(title);
            var json = JsonConvert.SerializeObject(new { authorWithPaymentTypeList }, new Newtonsoft.Json.Converters.StringEnumConverter());
            return Json(json);

        }





    }
}

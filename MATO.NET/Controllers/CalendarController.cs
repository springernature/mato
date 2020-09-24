using MATO.DataModel;
using MATO.NET.Persistence;
using MATO.NET.ViewModels;
using Newtonsoft.Json;
using System.Web.Mvc;

using System.Collections.Generic;

namespace MATO.NET.Controllers
{
    public class CalendarController : Controller
    {
        private readonly IUnitOfWork _unitOfWork;

        public CalendarController()
        {
            _unitOfWork = new UnitOfWork(new MATOContext());
        }

        // GET: Calendar
        public ActionResult Index()
        {
            CalendarViewModel model =
                new CalendarViewModel {Authors = _unitOfWork.UserService.GetApplicationUsersInRole("Author")};
            return View(model);
        }

        public ActionResult WebinarCalendar()
        {
            return View();
        }

        [HttpPost]
        public JsonResult GetAuthorCalendar(string id)
        {
            var author = _unitOfWork.RequestsService.GetAuthorDetail(id);
            var calendar = _unitOfWork.AdminService.GetCalendarForAuthor(id);

            var modifiedJsonString = "";

            if (calendar != null)
            {
                //saqibh add one day to end date for calendar otherwise it doesn't show properly
                dynamic jsonObject = Newtonsoft.Json.JsonConvert.DeserializeObject(calendar.Data);

                List<dynamic> newData = new List<dynamic>();

                foreach (var obj in jsonObject)
                {
                    System.DateTime endDateTime = System.Convert.ToDateTime(obj["end"]);
                    string endDate = endDateTime.Date.AddDays(1).ToString("yyyy-MM-dd");

                    obj.end = endDate;

                    newData.Add(obj);
                }

                modifiedJsonString = Newtonsoft.Json.JsonConvert.SerializeObject(newData);
                //end
            }

            //return Json(calendar == null ? "[]" : calendar.Data);
            return Json(calendar == null ? "[]" : modifiedJsonString);
        }

        [HttpPost]
        public JsonResult GetWebinarCalendar(string name)
        {
            var calendar = _unitOfWork.AdminService.GetWebinarCalendar(name);
            return Json(calendar == null ? "[]" : calendar.Data);
        }
    }
}
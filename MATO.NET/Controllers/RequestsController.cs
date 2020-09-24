using System;
using MATO.DataModel;
using MATO.NET.Persistence;
using MATO.NET.ViewModels;
using Microsoft.AspNet.Identity;
using Newtonsoft.Json;
using System.Web.Mvc;
using MATO.Classes;

namespace MATO.NET.Controllers
{
    public class RequestsController : Controller
    {
        private readonly IUnitOfWork _unitOfWork;

        public RequestsController()
        {
            _unitOfWork = new UnitOfWork(new MATOContext());
        }

        // GET: Requests
        //[Authorize] -- Only allow logged in Users when the application goes LIVE.
        public ActionResult Index()
        {
            return View();
        }

        [HttpGet]
        public ActionResult Submit()
        {
            SubmitRequestViewModel model = new SubmitRequestViewModel();
            model.CurrentUser = _unitOfWork.UserService.GetUserById(User.Identity.GetUserId());
            model.UserRegion = _unitOfWork.RequestsService.GetRegionById(model.CurrentUser.Region);

            EventViewModel eventModelOne = new EventViewModel();
            EventViewModel eventModelTwo = new EventViewModel();
            EventViewModel eventModelThree = new EventViewModel();

            model.Authors = _unitOfWork.UserService.GetApplicationUsersInRole("Author");
            model.PromotedTitle = _unitOfWork.RequestsService.GetPromotedTitleList();

            eventModelOne.EventType = _unitOfWork.RequestsService.GetEventTypes();
            eventModelOne.TargetSector = _unitOfWork.RequestsService.GetSectorsTargeted();
            eventModelOne.TalkType = _unitOfWork.RequestsService.GetTalkTypes();

            eventModelTwo.EventType = _unitOfWork.RequestsService.GetEventTypes();
            eventModelTwo.TargetSector = _unitOfWork.RequestsService.GetSectorsTargeted();
            eventModelTwo.TalkType = _unitOfWork.RequestsService.GetTalkTypes();

            eventModelThree.EventType = _unitOfWork.RequestsService.GetEventTypes();
            eventModelThree.TargetSector = _unitOfWork.RequestsService.GetSectorsTargeted();
            eventModelThree.TalkType = _unitOfWork.RequestsService.GetTalkTypes();

            //model.Country = _unitOfWork.RequestsService.GetCountryList();

            model.EventOne = eventModelOne;
            model.EventTwo = eventModelTwo;
            model.EventThree = eventModelThree;
            return View(model);
        }

        [HttpPost]
        public ActionResult Submit(SubmitRequestViewModel model)
        {
            var loggedIn = User.Identity.GetUserId();
            var user = _unitOfWork.UserService.GetUserById(loggedIn);
            var region = _unitOfWork.AdminService.GetRegionById(user.Region);
            var request = _unitOfWork.RequestsService.CreateRequest(model, user);

            if (model.EventOne.EventName != null)
            {
                var Event = _unitOfWork.RequestsService.CreateEvent(model.EventOne, request);
                var updatedRequest = _unitOfWork.RequestsService.UpdateRequestWithEvent(request, Event, 1);
                _unitOfWork.Complete();
            }

            if (model.EventTwo.EventName != null)
            {
                var Event = _unitOfWork.RequestsService.CreateEvent(model.EventTwo, request);
                var updatedRequest = _unitOfWork.RequestsService.UpdateRequestWithEvent(request, Event, 2);
                _unitOfWork.Complete();
            }

            if (model.EventThree.EventName != null)
            {
                var Event = _unitOfWork.RequestsService.CreateEvent(model.EventThree, request);
                var updatedRequest = _unitOfWork.RequestsService.UpdateRequestWithEvent(request, Event, 3);
                _unitOfWork.Complete();
            }

            var r = _unitOfWork.RequestsService.GetRequestById(request.Id);
            var manager = _unitOfWork.UserService.GetUserById(region.RegionalManagerId);
            _unitOfWork.EmailService.SendMail(manager, 1, r);

            return RedirectToAction("Index", "Home");
        }

        public ActionResult Manage()
        {
            ManageRequestViewModel model = new ManageRequestViewModel();
            var loggedIn = User.Identity.GetUserId();

            model.FinalisedRequests = _unitOfWork.RequestsService.GetFinalisedRequests(loggedIn);
            model.CancelledRequests = _unitOfWork.RequestsService.GetCancelledRequests(loggedIn);
            model.PendingRequests = _unitOfWork.RequestsService.GetPendingRequests(loggedIn);
            model.GetDeclinedRequests = _unitOfWork.RequestsService.GetDeclinedRequests(loggedIn);

            return View(model);
        }


        [HttpPost]
        public ActionResult GetAuthorsForTitles(string id)
        {
            var titleId = Int32.Parse(id);
            var authors = _unitOfWork.AdminService.GetAuthorsByTitleId(titleId);

            var json = JsonConvert.SerializeObject(new { authors }, new Newtonsoft.Json.Converters.StringEnumConverter());
            return Json(json);
        }

        public ActionResult Request(int id)
        {
            var request = _unitOfWork.RequestsService.GetRequestById(id);

            //saqibh
            //var loggedInUser = User.Identity.GetUserId();
            //var currentUser = _unitOfWork.UserService.GetUserById(loggedInUser);
            //var currentUserRegion = _unitOfWork.AdminService.GetRegionById(currentUser.Region);

            // Checks to see if you own the request, or are a decision maker, else 404.
            ViewRequestViewModel model = new ViewRequestViewModel
            {
                Author = _unitOfWork.RequestsService.GetAppUserFromId(request.RequestedAuthor.Id),
                PromotedTitle = _unitOfWork.RequestsService.GetPromotedTitleById(request.PromotedTitle.Id),
                Forecast = _unitOfWork.RequestsService.GetTitleForecastById(request.TitleForecast.Id),
                InboundDate = request.InboundDate,
                OutboundDate = request.OutboundDate,
                Inbound = request.Inbound,
                Outbound = request.Outbound,
                EventOne = request.EventOne,
                EventTwo = request.EventTwo,
                EventThree = request.EventThree,
                WhoSubmit = request.SubmitBy,
                SubmitDate = request.SubmitDate,
                ManagerApproved = request.ManagerApproval,
                AdminApproved = request.AdminApproval,
                ManagerApprovalDate = request.ManagerApprovalDate,
                AdminApprovalDate = request.AdminApprovalDate,
                Declined = request.Declined,
                DeclinedDate = request.DeclinedDate,
                Finalised = request.Finalised,
                FinalisedDate = request.FinalisedDate,
                AuthorNotesBySalesRep = request.AuthorNotesBySalesRep,
                NonAuthorNotesBySalesRep = request.NonAuthorNotesBySalesRep,
                Region = request.Region,
            };

            return View(model);
        }

        public JsonResult GetAuthorDetails(string id)
        {
            var author = _unitOfWork.RequestsService.GetAuthorDetail(id);
            var authorTitles = _unitOfWork.RequestsService.GetAssociatedTitles(author);
            var calendar = _unitOfWork.UserService.GetUserCalendarByUser(author);
            var aud = _unitOfWork.UserService.GetAuthorDetails(id);
            if(calendar == null) { calendar = new Calendar(); }
            var json = JsonConvert.SerializeObject(new
            {
                Author = author,
                Titles = authorTitles,
                Calendar = calendar,
                Aud = aud
            }, new Newtonsoft.Json.Converters.StringEnumConverter());
            return Json(json);
        }

        [HttpGet]
        public ActionResult Webinar()
        {
            SubmitWebinarRequestViewModel model = new SubmitWebinarRequestViewModel();
            model.CurrentUser = _unitOfWork.UserService.GetUserById(User.Identity.GetUserId());
            model.UserRegion = _unitOfWork.RequestsService.GetRegionById(model.CurrentUser.Region);

            EventViewModel eventModelOne = new EventViewModel();

            model.Authors = _unitOfWork.UserService.GetApplicationUsersInRole("Author");
            model.PromotedTitle = _unitOfWork.RequestsService.GetPromotedTitleList();

            eventModelOne.EventType = _unitOfWork.RequestsService.GetEventTypes();
            eventModelOne.TargetSector = _unitOfWork.RequestsService.GetSectorsTargeted();
            eventModelOne.TalkType = _unitOfWork.RequestsService.GetTalkTypes();

            model.EventOne = eventModelOne;
            return View(model);
        }

        [HttpPost]
        public ActionResult Webinar(SubmitWebinarRequestViewModel model)
        {
            var loggedIn = User.Identity.GetUserId();
            var user = _unitOfWork.UserService.GetUserById(loggedIn);
            var region = _unitOfWork.AdminService.GetRegionById(user.Region);
            var request = _unitOfWork.RequestsService.CreateWebinar(model, user);
            var r = _unitOfWork.RequestsService.GetRequestById(request.Id);

            if (model.EventOne.EventName != null)
            {
                var Event = _unitOfWork.RequestsService.CreateEvent(model.EventOne, r);
                var updatedRequest = _unitOfWork.RequestsService.UpdateRequestWithEvent(r, Event, 1);
                _unitOfWork.Complete();
            }
            
            var manager = _unitOfWork.UserService.GetUserById(region.RegionalManagerId);
            _unitOfWork.RequestsService.AddRequestToWebinarCalendar("Webinar Room 1", r.EventOne.EventDate, request);
            _unitOfWork.Complete();

            return RedirectToAction("Index", "Home");
        }

        [HttpPost]
        public ActionResult GetWebinarCalendar(string id)
        {
            var calendar = _unitOfWork.UserService.GetWebinarCalendar();
           
            var json = JsonConvert.SerializeObject(new
            {
                Calendar = calendar
            }, new Newtonsoft.Json.Converters.StringEnumConverter());
            return Json(json);
        }
    }
}
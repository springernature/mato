using System.Web.Mvc;
using MATO.Classes;
using MATO.DataModel;
using MATO.NET.Helpers;
using MATO.NET.Persistence;
using MATO.NET.ViewModels;
using Microsoft.AspNet.Identity;

namespace MATO.NET.Controllers
{
    public class DecisionsController : Controller
    {
        private readonly IUnitOfWork _unitOfWork;

        public DecisionsController()
        {
            _unitOfWork = new UnitOfWork(new MATOContext());
        }

        [HttpGet]
        public ActionResult Index()
        {
            PendingDecisionsViewModel model = new PendingDecisionsViewModel();
            var loggedIn = User.Identity.GetUserId();
            var manager = _unitOfWork.DecisionsService.IsLoggedInUserRegionManager(loggedIn); // Returns Region if yes. 
            var userRole = _unitOfWork.UserService.GetUserRoleByUserId(loggedIn);

            model.Decisions =
                manager != null
                    ? _unitOfWork.DecisionsService.GetRequestsByRegion(manager)
                    : null; // Only open requests. Closed are not returned here.

            //Moving this to a report
            //model.ClosedDecisions = _unitOfWork.DecisionsService.GetMyClosedDecisions(loggedIn);

            model.AdminDecisions =
                userRole == "SuperAdmin"
                    ? _unitOfWork.DecisionsService.GetRequestsForAdministrator()
                    : null; // Only open requests. Closed are not returned here.

            model.AuthorDecisions =
                userRole == "Author"
                    ? _unitOfWork.DecisionsService.GetRequestsForAuthor(loggedIn)
                    : null; // Only non-finalised deals, with positive manager and admin decisions are returned here. 

            return View(model);
        }

        public ActionResult Decision(int id)
        {
            var request = _unitOfWork.RequestsService.GetRequestById(id);
            var loggedIn = User.Identity.GetUserId();
            var role = _unitOfWork.UserService.GetUserRoleByUserId(loggedIn);

            switch (role)
            {
                case "SuperAdmin":
                    AdminDecision(id);
                    break;
                case "SalesRep":
                    return RedirectToAction("Request", "Requests", new { id = id });
                case "Admin":
                    return RedirectToAction("Index", "Requests");
                case "Author":
                    AuthorDecision(id);
                    break;
            }

            if (request.Region.RegionalManagerId == loggedIn)
            {
                // Checks to see if you own the request, or are a decision maker, else 404.
                ViewRequestViewModel model = new ViewRequestViewModel
                {
                    Id = request.Id,
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
                    Region = request.Region,
                    WhoSubmit = request.SubmitBy,
                    SubmitDate = request.SubmitDate,
                    AuthorNotesBySalesRep = request.AuthorNotesBySalesRep,
                    NonAuthorNotesBySalesRep = request.NonAuthorNotesBySalesRep
                };

                return View(model);
            }

            return HttpNotFound();
        }

        public ActionResult AdminDecision(int id)
        {
            var request = _unitOfWork.RequestsService.GetRequestById(id);
            var loggedIn = User.Identity.GetUserId();
            var role = _unitOfWork.UserService.GetUserRoleByUserId(loggedIn);

            switch (role)
            {
                case "SalesRep":
                    return RedirectToAction("Request", "Requests", new { id = id });
                case "Admin":
                    return RedirectToAction("Index", "Requests");
                case "Author":
                    AuthorDecision(id);
                    break;
                case "Manager":
                    Decision(id);
                    break;
            }

            // Checks to see if you own the request, or are a decision maker, else 404.
            ViewRequestViewModel model = new ViewRequestViewModel
            {
                Id = request.Id,
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
                ManagerApprovalDate = request.ManagerApprovalDate,
                AuthorNotesBySalesRep = request.AuthorNotesBySalesRep,
                NonAuthorNotesBySalesRep = request.NonAuthorNotesBySalesRep
            };

            return View(model);
        }

        public ActionResult AuthorDecision(int id)
        {
            var request = _unitOfWork.RequestsService.GetRequestById(id);
            var loggedIn = User.Identity.GetUserId();
            var role = _unitOfWork.UserService.GetUserRoleByUserId(loggedIn);

            switch (role)
            {
                case "SuperAdmin":
                    AdminDecision(id);
                    break;
                case "SalesRep":
                    return RedirectToAction("Request", "Requests", new { id = id });
                case "Admin":
                    return RedirectToAction("Index", "Requests");
                case "Manager":
                    Decision(id);
                    break;
            }

            // Checks to see if you own the request, or are a decision maker, else 404.
            ViewRequestViewModel model = new ViewRequestViewModel
            {
                Id = request.Id,
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
                AuthorNotesBySalesRep = request.AuthorNotesBySalesRep
            };

            return View(model);
        }

        [HttpPost]
        [ButtonHelper.MultipleButtonAttribute(Name = "action", Argument = "Approve")]
        public ActionResult Approve(ViewRequestViewModel model)
        {
            _unitOfWork.DecisionsService.ApproveRequest(model.Id);
            _unitOfWork.Complete();

            var request = _unitOfWork.RequestsService.GetRequestById(model.Id);
            var whoSubmit = _unitOfWork.UserService.GetUserById(request.SubmitBy.Id);

            _unitOfWork.EmailService.SendMail(whoSubmit, 3, request);

            return RedirectToAction("Index", "Decisions");
        }

        [HttpPost]
        [ButtonHelper.MultipleButtonAttribute(Name = "action", Argument = "Decline")]
        public ActionResult Decline(ViewRequestViewModel model)
        {
            _unitOfWork.DecisionsService.DeclineRequest(model.Id);
            _unitOfWork.Complete();

            var request = _unitOfWork.RequestsService.GetRequestById(model.Id);
            var whoSubmit = _unitOfWork.UserService.GetUserById(request.SubmitBy.Id);

            _unitOfWork.EmailService.SendMail(whoSubmit, 2, request);
            return RedirectToAction("Index", "Decisions");
        }

        [HttpPost]
        [ButtonHelper.MultipleButtonAttribute(Name = "action", Argument = "AdminApprove")]
        public ActionResult AdminApprove(ViewRequestViewModel model)
        {
            _unitOfWork.DecisionsService.AdminApproveRequest(model.Id);
            _unitOfWork.Complete();

            var request = _unitOfWork.RequestsService.GetRequestById(model.Id);
            var whoSubmit = _unitOfWork.UserService.GetUserById(request.SubmitBy.Id);
            var region = _unitOfWork.AdminService.GetRegionById(whoSubmit.Region);
            var manager = _unitOfWork.UserService.GetUserById(region.RegionalManagerId);
            var author = _unitOfWork.UserService.GetUserById(request.RequestedAuthor.Id);

            _unitOfWork.RequestsService.AddRequestToAuthorCalendar(request.RequestedAuthor.Id, request.EventOne.EventName, request.OutboundDate, request.InboundDate, request.Id);
            _unitOfWork.Complete();

            _unitOfWork.EmailService.SendMail(manager, 4, request); // Regional Manager
            _unitOfWork.EmailService.SendMail(whoSubmit, 6, request); // Sales Rep Who Submit
            _unitOfWork.EmailService.SendMail(author, 8, request); // Sales Rep Who Submit

            return RedirectToAction("Index", "Decisions");
        }

        [HttpPost]
        [ButtonHelper.MultipleButtonAttribute(Name = "action", Argument = "AdminDecline")]
        public ActionResult AdminDecline(ViewRequestViewModel model)
        {
            _unitOfWork.DecisionsService.AdminDeclineRequest(model.Id);
            _unitOfWork.Complete();

            var request = _unitOfWork.RequestsService.GetRequestById(model.Id);
            var whoSubmit = _unitOfWork.UserService.GetUserById(request.SubmitBy.Id);
            var region = _unitOfWork.AdminService.GetRegionById(whoSubmit.Region);
            var manager = _unitOfWork.UserService.GetUserById(region.RegionalManagerId);

            _unitOfWork.EmailService.SendMail(manager, 5, request); // Regional Manager
            _unitOfWork.EmailService.SendMail(whoSubmit, 7, request); // Sales Rep Who Submit

            return RedirectToAction("Index", "Decisions");
        }

        [HttpPost]
        [ButtonHelper.MultipleButtonAttribute(Name = "action", Argument = "AuthorApprove")]
        public ActionResult AuthorApprove(ViewRequestViewModel model)
        {
            _unitOfWork.DecisionsService.AuthorApproveRequest(model.Id);
            _unitOfWork.Complete();

            var request = _unitOfWork.RequestsService.GetRequestById(model.Id);
            var whoSubmit = _unitOfWork.UserService.GetUserById(request.SubmitBy.Id);
            var region = _unitOfWork.AdminService.GetRegionById(whoSubmit.Region);
            var manager = _unitOfWork.UserService.GetUserById(region.RegionalManagerId);
            var admin = _unitOfWork.UserService.GetSuperAdmin("SuperAdmin"); // Make non static // Get Admin Email. 

            _unitOfWork.EmailService.SendMail(admin, 9, request); // Super Admin
            _unitOfWork.EmailService.SendMail(manager, 10, request); // Regional Manager
            _unitOfWork.EmailService.SendMail(whoSubmit, 11, request); // Sales Rep Who Submit

            return RedirectToAction("Index", "Decisions");
        }

        [HttpPost]
        [ButtonHelper.MultipleButtonAttribute(Name = "action", Argument = "AuthorDecline")]
        public ActionResult AuthorDecline(ViewRequestViewModel model)
        {
            _unitOfWork.DecisionsService.AuthorDeclineRequest(model.Id);
            _unitOfWork.Complete();

            var request = _unitOfWork.RequestsService.GetRequestById(model.Id);
            var whoSubmit = _unitOfWork.UserService.GetUserById(request.SubmitBy.Id);
            var region = _unitOfWork.AdminService.GetRegionById(whoSubmit.Region);
            var manager = _unitOfWork.UserService.GetUserById(region.RegionalManagerId);
            var admin = _unitOfWork.UserService.GetSuperAdmin("SuperAdmin"); // Make non static // Get Admin Email. 

            _unitOfWork.EmailService.SendMail(admin, 12, request); // Super Admin
            _unitOfWork.EmailService.SendMail(manager, 13, request); // Regional Manager
            _unitOfWork.EmailService.SendMail(whoSubmit, 14, request); // Sales Rep Who Submit

            return RedirectToAction("Index", "Decisions");
        }
    }
}
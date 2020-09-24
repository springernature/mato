using System;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using MATO.Classes;
using MATO.DataModel;
using MATO.NET.Persistence;
using MATO.NET.ViewModels;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.Owin;
using Newtonsoft.Json.Linq;
using System.Web.Script.Serialization;
using System.Collections.Generic;
using System.Linq;
using System.Net.Mail;
using MATO.NET.Models;
using Microsoft.AspNet.Identity.EntityFramework;
using Newtonsoft.Json;

namespace MATO.NET.Controllers
{
    [Authorize(Roles = "Admin, SuperAdmin")]
    public class AdminController : Controller
    {
        private readonly IUnitOfWork _unitOfWork;
        private ApplicationSignInManager _signInManager;
        private ApplicationUserManager _userManager;

        public AdminController()
        {
            _unitOfWork = new UnitOfWork(new MATOContext());
        }

        public AdminController(ApplicationUserManager userManager, ApplicationSignInManager signInManager)
        {
            UserManager = userManager;
            SignInManager = signInManager;
        }

        public ApplicationSignInManager SignInManager
        {
            get { return _signInManager ?? HttpContext.GetOwinContext().Get<ApplicationSignInManager>(); }
            private set { _signInManager = value; }
        }

        public ApplicationUserManager UserManager
        {
            get { return _userManager ?? HttpContext.GetOwinContext().GetUserManager<ApplicationUserManager>(); }
            private set { _userManager = value; }
        }

        // GET: Admin
        public ActionResult Index()
        {
            return View();
        }

        [HttpGet]
        public ActionResult CreateTitle()
        {
            CreateTitleViewModel model = new CreateTitleViewModel
            {
                TargetSector = _unitOfWork.RequestsService.GetSectorsTargeted(),
                Authors = _unitOfWork.AdminService.GetAuthorsForMultiSelect(),
            };
            return View(model);
        }

        [HttpPost]
        public ActionResult CreateTitle(CreateTitleViewModel model)
        {
            var createTitle = _unitOfWork.AdminService.CreateTitle(model);
            _unitOfWork.Complete();
            return RedirectToAction("Index", "Admin");
        }

        [HttpGet]
        public ActionResult ModifyTitle()
        {
            ModifyTitleViewModel model = new ModifyTitleViewModel
            {
                Titles = _unitOfWork.RequestsService.GetPromotedTitleList(),
                TargetSector = _unitOfWork.RequestsService.GetSectorsTargeted(),
                Authors = _unitOfWork.AdminService.GetAuthorsForMultiSelect(),
             
            };

            return View(model);
        }

        [HttpPost]
        public JsonResult ModifyTitle(string id)
        {
            var titleId = Int32.Parse(id);
            var title = _unitOfWork.AdminService.GetTitleById(titleId);
            var authors = _unitOfWork.AdminService.GetAuthorsByTitleId(titleId);
            var authorwithpaymenttype = _unitOfWork.AdminService.GetAuthorsAndPaymenetType(titleId);
            var json = JsonConvert.SerializeObject(new { title, authors, authorwithpaymenttype }, new Newtonsoft.Json.Converters.StringEnumConverter());
            return Json(json);
        }

        [HttpPost]
        public JsonResult UpdateTitle(string Id, string Name, string TargetSector, object Authors)
        {
            var titleId = Int32.Parse(Id);
            var sector = Int32.Parse(TargetSector);
            var title = _unitOfWork.AdminService.GetTitleById(titleId);
            _unitOfWork.AdminService.UpdateTitle(title, Name, sector, Authors.ToString());

            return Json("");
        }

        [HttpGet]
        public ActionResult CreateUser()
        {
            CreateUserViewModel model = new CreateUserViewModel
            {
                Region = _unitOfWork.AdminService.GetRegions(),
                Roles = _unitOfWork.AdminService.GetUserRoles()
            };

            return View(model);
        }

        [HttpPost]
        public async Task<JsonResult> CreateUser(string UserId, string FirstName, string LastName, string Email, int Region, string Country,
            string Role, string AddressLine1, string AddressLine2, string AddressLine3, string AuthorType, string AuthorNotes)
        {

            if (FirstName == null || LastName == null || Email == null || Region == 0 || string.IsNullOrEmpty(Role))
            {
                var json = JsonConvert.SerializeObject("Missing", new Newtonsoft.Json.Converters.StringEnumConverter());
                return Json(json);
            }
            var userCheck = _unitOfWork.UserService.GetUserByEmailAddress(Email);
            if (userCheck != null)
            {
                var json = JsonConvert.SerializeObject("Email", new Newtonsoft.Json.Converters.StringEnumConverter());
                return Json(json);
            }

            var region = _unitOfWork.AdminService.GetRegionById(Region);
            var role = _unitOfWork.AdminService.GetRoleByName(Role);
            var user = new AppUser { UserName = Email, Email = Email, Region = region.Id, FirstName = FirstName, LastName = LastName, FullName = FirstName + " " + LastName, Country = Country };
            var result = await UserManager.CreateAsync(user, "Welcome.01!");

            if (!result.Succeeded)
            {
                var json = JsonConvert.SerializeObject("Creation", new Newtonsoft.Json.Converters.StringEnumConverter());
                return Json(json);
            }
            else
            {
                if (role.Name == "Author")
                {
                    AuthorDetails aud = new AuthorDetails
                    {
                        AddressLine1 = AddressLine1,
                        AddressLine2 = AddressLine2,
                        AddressLine3 = AddressLine3,
                        AuthorNotes = AuthorNotes,
                        AuthorType = AuthorType,
                        AuthorId = user.Id
                    };

                    _unitOfWork.UserService.CreateAuthorDetailsEntry(aud);
                    _unitOfWork.Complete();
                }


                var code = await UserManager.GeneratePasswordResetTokenAsync(user.Id);
                var callbackUrl = Url.Action("ResetPassword", "Account", new { userId = user.Id, code = code }, protocol: Request.Url.Scheme);

                //Uncomment below when ready to deploy
                //_unitOfWork.EmailService.NewUserMail(user, "Welcome.01!", role.Name, callbackUrl);

                //Assign the new User to his role, selected on the form
                UserManager.AddToRole(user.Id, role.Name);

                var json = JsonConvert.SerializeObject("Success", new Newtonsoft.Json.Converters.StringEnumConverter());
                return Json(json);
            }
        }

        [HttpGet]
        public ActionResult ModifyUser()
        {
            ModifyUserViewModel model = new ModifyUserViewModel
            {
                Users = _unitOfWork.UserService.GetAllUsers(),
                Region = _unitOfWork.AdminService.GetRegions(),
                Roles = _unitOfWork.AdminService.GetUserRoles()
            };

            return View(model);
        }

        public JsonResult UpdateUserDetails(string UserId, string FirstName, string LastName, string Email, int Region, string Country,
            string Role, string AddressLine1, string AddressLine2, string AddressLine3, string AuthorType, string AuthorNotes)
        {
            var user = _unitOfWork.UserService.GetUserById(UserId);
            var userRoles = UserManager.GetRoles(user.Id);
            UserManager.RemoveFromRoles(user.Id, userRoles.ToArray());
            UserManager.AddToRole(user.Id, Role);
            var updatedUser = _unitOfWork.UserService.UpdateUser(user, FirstName, LastName, Email, Region, Country, Role);

            if (AddressLine1 != null)
            {
                var aud = _unitOfWork.UserService.GetAuthorDetails(user.Id);
                if (aud != null)
                {
                    aud.AddressLine1 = AddressLine1;
                    aud.AddressLine2 = AddressLine2;
                    aud.AddressLine3 = AddressLine3;
                    aud.AuthorType = AuthorType;
                    aud.AuthorNotes = AuthorNotes;
                    aud.AuthorId = UserId;
                }
                else
                {
                    AuthorDetails newAud = new AuthorDetails
                    {
                        AddressLine1 = AddressLine1,
                        AddressLine2 = AddressLine2,
                        AddressLine3 = AddressLine3,
                        AuthorType = AuthorType,
                        AuthorNotes = AuthorNotes,
                        AuthorId = UserId
                    };

                    _unitOfWork.UserService.CreateAuthorDetailsEntry(newAud);
                }

                _unitOfWork.Complete();

            }

            var json = JsonConvert.SerializeObject(updatedUser, new Newtonsoft.Json.Converters.StringEnumConverter());
            return Json(json);
        }

        [HttpPost]
        public JsonResult GetUserDetails(string id)
        {
            var user = _unitOfWork.UserService.GetUserById(id);
            var role = _unitOfWork.UserService.GetUserRoleByUserId(id);
            var aud = _unitOfWork.UserService.GetAuthorDetails(id);

            var json = JsonConvert.SerializeObject(new { user, role, aud }, new Newtonsoft.Json.Converters.StringEnumConverter());
            return Json(json);
        }

        [HttpPost]
        public JsonResult DeleteUser(string userId)
        {
            var user = _unitOfWork.UserService.GetUserById(userId);
            var logins = user.Logins;
            var userRoles = UserManager.GetRoles(user.Id);

            if (logins.Any())
            {
                foreach (var login in logins.ToList())
                {
                    _userManager.RemoveLogin(login.UserId, new UserLoginInfo(login.LoginProvider, login.ProviderKey));
                }
            }

            if (userRoles.Any())
            {
                UserManager.RemoveFromRoles(user.Id, userRoles.ToArray());
            }

            _unitOfWork.UserService.DeleteUser(userId);
            _unitOfWork.Complete();

            var json = JsonConvert.SerializeObject("True", new Newtonsoft.Json.Converters.StringEnumConverter());
            return Json(json);
        }

        [HttpGet]
        public ActionResult CreateRegion()
        {
            CreateRegionViewModel model = new CreateRegionViewModel()
            {
                RegionalManager = _unitOfWork.UserService.GetApplicationUsersInRole("Manager")
            };

            return View(model);
        }

        [HttpPost]
        public ActionResult CreateRegion(CreateRegionViewModel model)
        {
            _unitOfWork.AdminService.CreateRegion(model);
            _unitOfWork.Complete();
            return RedirectToAction("Index", "Admin");
        }

        [HttpGet]
        public ActionResult CreateCalendarEvent()
        {
            CreateCalendarEventViewModel model = new CreateCalendarEventViewModel
            {
                Authors = _unitOfWork.UserService.GetApplicationUsersInRole("Author")
            };
            return View(model);
        }

        [HttpPost]
        public ActionResult CreateCalendarEvent(CreateCalendarEventViewModel model)
        {
            var calendar = _unitOfWork.AdminService.GetAuthorCalendar(model.SelectedAuthorId);

            if (calendar != null)
            {
                JavaScriptSerializer jsSerializer = new JavaScriptSerializer();
                var cevents = jsSerializer.Deserialize<List<CalendarEvent>>(calendar.Data);

                var c = new CalendarEvent
                {
                    title = model.EventName,
                    start = ((DateTime)model.StartDate).ToString("yyyy-MM-dd"),
                    end = ((DateTime)model.EndDate).ToString("yyyy-MM-dd")
                };

                cevents.Add(c);

                var json = new JavaScriptSerializer().Serialize(cevents);
                _unitOfWork.AdminService.UpdateCalendarForAuthor(model.SelectedAuthorId, json);
                _unitOfWork.Complete();

                return RedirectToAction("Index", "Admin");
            }
            else
            {
                var c = new CalendarEvent
                {
                    title = model.EventName,
                    start = ((DateTime)model.StartDate).ToString("yyyy-MM-dd"),
                    end = ((DateTime)model.EndDate).ToString("yyyy-MM-dd")
                };

                var json = new JavaScriptSerializer().Serialize(c);
                _unitOfWork.AdminService.CreateCalendarForAuthor(model.SelectedAuthorId, json);
                _unitOfWork.Complete();

                return RedirectToAction("Index", "Admin");
            }
        }

        [HttpGet]
        public ActionResult EmailList()
        {
            EmailListViewModel model = new EmailListViewModel();

            model.EmailTemplates = _unitOfWork.AdminService.GetEmailTemplates();
            return View(model);
        }

        [HttpGet]
        public ActionResult ModifyRegion()
        {
            ModifyRegionViewModel model = new ModifyRegionViewModel
            {
                Regions = _unitOfWork.AdminService.GetRegions(),
                RegionalManager = _unitOfWork.UserService.GetApplicationUsersInRole("Manager"),
            };

            return View(model);
        }

        [HttpPost]
        public ActionResult ModifyRegion(ModifyRegionViewModel model)
        {
            var region = _unitOfWork.AdminService.UpdateRegion(model);
            _unitOfWork.Complete();

            return RedirectToAction("Index", "Admin");
        }

        public JsonResult GetRegion(string id)
        {
            var regionId = Int32.Parse(id);
            var region = _unitOfWork.AdminService.GetRegionById(regionId);

            var json = JsonConvert.SerializeObject(region, new Newtonsoft.Json.Converters.StringEnumConverter());
            return Json(json);
        }

        public JsonResult GetEmailList(string id)
        {
            var emailId = Int32.Parse(id);
            var email = _unitOfWork.AdminService.GetEmailTemplateById(emailId);

            var json = JsonConvert.SerializeObject(email, new Newtonsoft.Json.Converters.StringEnumConverter());
            return Json(json);
        }

        [HttpGet]
        public ActionResult ModifyCalendarEvents()
        {
            ModifyCalendarEventsViewModel model = new ModifyCalendarEventsViewModel
            {
                Users = _unitOfWork.UserService.GetAllUsers()
            };

            return View(model);
        }

        [HttpPost]
        public JsonResult DeleteCalendarEvent(string Title, string Id)
        {
            var user = _unitOfWork.UserService.GetUserById(Id);
            var newCalendar = _unitOfWork.AdminService.DeleteCalendarEvent(Title, user);
            _unitOfWork.Complete();

            return Json(newCalendar.Data);
        }

        [HttpPost]
        public JsonResult GetUserCalendar(string id)
        {
            var user = _unitOfWork.UserService.GetUserById(id);
            var calendar = _unitOfWork.UserService.GetUserCalendarByUser(user);

            if (calendar != null)
            {
                return Json(calendar.Data);
            }

            return Json("[]");

        }

        [HttpGet]
        public ActionResult ModifyEventTypes()
        {
            AdminModifySystemViewModel model =
                new AdminModifySystemViewModel { EventTypes = _unitOfWork.AdminService.GetEventTypes() };

            return View(model);
        }

        public JsonResult AddEventType(string name)
        {
            var eventType = _unitOfWork.AdminService.AddEventType(name);
            _unitOfWork.Complete();

            var json = JsonConvert.SerializeObject(eventType, new Newtonsoft.Json.Converters.StringEnumConverter());
            return Json(json);
        }

        public JsonResult ToggleActiveEventType(string id)
        {
            var eventId = Int32.Parse(id);
            var eventType = _unitOfWork.AdminService.ToggleEventType(eventId);
            _unitOfWork.Complete();

            var json = JsonConvert.SerializeObject(eventType, new Newtonsoft.Json.Converters.StringEnumConverter());
            return Json(json);
        }

        [HttpGet]
        public ActionResult ModifyTargetSectors()
        {
            AdminModifySystemViewModel model =
                new AdminModifySystemViewModel { TargetSectors = _unitOfWork.RequestsService.GetSectorsTargeted() };

            return View(model);
        }

        [HttpGet]
        public ActionResult ModifyTalkTypes()
        {
            AdminModifySystemViewModel model =
                new AdminModifySystemViewModel { TalkTypes = _unitOfWork.RequestsService.GetTalkTypes() };

            return View(model);
        }

        public JsonResult AddTargetSector(string name)
        {
            var targetSector = _unitOfWork.AdminService.AddTargetSector(name);
            _unitOfWork.Complete();

            var json = JsonConvert.SerializeObject(targetSector, new Newtonsoft.Json.Converters.StringEnumConverter());
            return Json(json);
        }

        public JsonResult ToggleActiveTargetSector(string id)
        {
            var targetId = Int32.Parse(id);
            var targetSector = _unitOfWork.AdminService.ToggleTargetSector(targetId);
            _unitOfWork.Complete();

            var json = JsonConvert.SerializeObject(targetSector, new Newtonsoft.Json.Converters.StringEnumConverter());
            return Json(json);
        }

        public JsonResult AddTalkType(string name)
        {
            var talkType = _unitOfWork.AdminService.AddTalkType(name);
            _unitOfWork.Complete();

            var json = JsonConvert.SerializeObject(talkType, new Newtonsoft.Json.Converters.StringEnumConverter());
            return Json(json);
        }

        public JsonResult ToggleActiveTalkType(string id)
        {
            var talkTypeId = Int32.Parse(id);
            var talkType = _unitOfWork.AdminService.ToggleTalkType(talkTypeId);
            _unitOfWork.Complete();

            var json = JsonConvert.SerializeObject(talkType, new Newtonsoft.Json.Converters.StringEnumConverter());
            return Json(json);
        }
        public JsonResult UpdateEmailTemplate(string id, string html)
        {
            var emailTemplateId = Int32.Parse(id);
            var emailTemplate = _unitOfWork.AdminService.GetEmailTemplateById(emailTemplateId);
            emailTemplate.Html = html;
            _unitOfWork.Complete();

            return Json("Success");
        }

        [HttpGet]
        public ActionResult WelcomeEmail()
        {
            ModifyUserViewModel model = new ModifyUserViewModel
            {
                Users = _unitOfWork.UserService.GetAllUsers(),
                Region = null,
                Roles = null

            };

            return View(model);
        }

        public JsonResult WelcomeEmail(string Id)
        {
            var u = _unitOfWork.UserService.GetUserById(Id);
            var role = _unitOfWork.UserService.GetUserRoleByUserId(Id);

            string baseUrl = Request.Url.Scheme + "://" + Request.Url.Authority +
                             Request.ApplicationPath.TrimEnd('/') + "/";

            var email = _unitOfWork.EmailService.SendWelcomeEmail(u, "Welcome.01!", role, baseUrl);

            if (email)
            {
                return Json("SUCCESS");
            }

            return Json("FAIL");

        }

        [HttpGet]
        public ActionResult ModifyRequest()
        {
            List<Requests> req = _unitOfWork.RequestsService.GetAllRequests();
            return View(req);
        }

        [HttpGet]
        public ActionResult ModifyRequestDetail(int id)
        {
            var request = _unitOfWork.RequestsService.GetRequestById(id);

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
                NonAuthorNotesBySalesRep = request.NonAuthorNotesBySalesRep
            };

            return View(model);
        }

        [HttpPost]
        public ActionResult UpdateRequest(SubmitRequestViewModel model)
        {
            var request = _unitOfWork.RequestsService.GetRequestById(model.Id); // Pass Id to get Request.

            if (request.TitleForecast.Year1 != model.Forecast.Year1)
            {
                _unitOfWork.AdminService.CreateAuditLog(request.Id, "Forecast Year 1", request.TitleForecast.Year1.ToString(),
                    model.Forecast.Year1.ToString(), "Super Admin Panel");
                request.TitleForecast.Year1 = model.Forecast.Year1;
                _unitOfWork.Complete();
            }

            if (request.TitleForecast.Year2 != model.Forecast.Year2)
            {
                _unitOfWork.AdminService.CreateAuditLog(request.Id, "Forecast Year 2", request.TitleForecast.Year2.ToString(),
                    model.Forecast.Year2.ToString(), "Super Admin Panel");
                request.TitleForecast.Year2 = model.Forecast.Year2;
                _unitOfWork.Complete();
            }

            if (request.TitleForecast.Year3 != model.Forecast.Year3)
            {
                _unitOfWork.AdminService.CreateAuditLog(request.Id, "Forecast Year 3", request.TitleForecast.Year3.ToString(),
                    model.Forecast.Year3.ToString(), "Super Admin Panel");
                request.TitleForecast.Year3 = model.Forecast.Year3;
                _unitOfWork.Complete();
            }

            if (request.Outbound != model.Outbound)
            {
                _unitOfWork.AdminService.CreateAuditLog(request.Id, "Outbound", request.Outbound,
                    model.Outbound, "Super Admin Panel");
                request.Outbound = model.Outbound;
                _unitOfWork.Complete();
            }

            if (request.Inbound != model.Inbound)
            {
                _unitOfWork.AdminService.CreateAuditLog(request.Id, "Inbound", request.Inbound,
                    model.Inbound, "Super Admin Panel");
                request.Inbound = model.Inbound;
                _unitOfWork.Complete();
            }

            if (request.OutboundDate != model.OutboundDate)
            {
                _unitOfWork.AdminService.CreateAuditLog(request.Id, "Outbound Date", request.OutboundDate.ToString(),
                    model.OutboundDate.ToString(), "Super Admin Panel");
                request.OutboundDate = model.OutboundDate;
                _unitOfWork.Complete();
            }

            if (request.InboundDate != model.InboundDate)
            {
                _unitOfWork.AdminService.CreateAuditLog(request.Id, "Outbound Date", request.OutboundDate.ToString(),
                    model.OutboundDate.ToString(), "Super Admin Panel");
                request.OutboundDate = model.OutboundDate;
                _unitOfWork.Complete();
            }

            if (request.EventOne.EventName != model.EventOne.EventName)
            {
                _unitOfWork.AdminService.CreateAuditLog(request.Id, "EventOne Name", request.EventOne.EventName,
                    model.EventOne.EventName, "Super Admin Panel");
                request.EventOne.EventName = model.EventOne.EventName;
                _unitOfWork.Complete();
            }

            if (request.EventOne.EventCity != model.EventOne.EventCity)
            {
                _unitOfWork.AdminService.CreateAuditLog(request.Id, "EventOne City", request.EventOne.EventCity,
                    model.EventOne.EventCity, "Super Admin Panel");
                request.EventOne.EventCity = model.EventOne.EventCity;
                _unitOfWork.Complete();
            }

            if (model.EventTwo != null)
            {
                if (request.EventTwo.EventName != model.EventTwo.EventName)
                {
                    _unitOfWork.AdminService.CreateAuditLog(request.Id, "EventTwo Name", request.EventTwo.EventName,
                        model.EventTwo.EventName, "Super Admin Panel");
                    request.EventTwo.EventName = model.EventTwo.EventName;
                    _unitOfWork.Complete();
                }

                if (request.EventTwo.EventCity != model.EventTwo.EventCity)
                {
                    _unitOfWork.AdminService.CreateAuditLog(request.Id, "EventTwo City", request.EventTwo.EventCity,
                        model.EventTwo.EventCity, "Super Admin Panel");
                    request.EventTwo.EventCity = model.EventTwo.EventCity;
                    _unitOfWork.Complete();
                }
            }

            if (model.EventThree != null)
            {
                if (request.EventThree.EventName != model.EventThree.EventName)
                {
                    _unitOfWork.AdminService.CreateAuditLog(request.Id, "EventThree Name", request.EventThree.EventName,
                        model.EventThree.EventName, "Super Admin Panel");
                    request.EventThree.EventName = model.EventThree.EventName;
                    _unitOfWork.Complete();
                }

                if (request.EventThree.EventCity != model.EventThree.EventCity)
                {
                    _unitOfWork.AdminService.CreateAuditLog(request.Id, "EventThree City", request.EventThree.EventCity,
                        model.EventThree.EventCity, "Super Admin Panel");
                    request.EventThree.EventCity = model.EventThree.EventCity;
                    _unitOfWork.Complete();
                }
            }

            if (request.AuthorNotesBySalesRep != model.AuthorNotesBySalesRep)
            {
                _unitOfWork.AdminService.CreateAuditLog(request.Id, "Author Notes", request.AuthorNotesBySalesRep,
                    model.AuthorNotesBySalesRep, "Super Admin Panel");
                request.AuthorNotesBySalesRep = model.AuthorNotesBySalesRep;
                _unitOfWork.Complete();
            }

            if (request.NonAuthorNotesBySalesRep != model.NonAuthorNotesBySalesRep)
            {
                _unitOfWork.AdminService.CreateAuditLog(request.Id, "NonAuthor Notes", request.NonAuthorNotesBySalesRep,
                    model.NonAuthorNotesBySalesRep, "Super Admin Panel");
                request.NonAuthorNotesBySalesRep = model.NonAuthorNotesBySalesRep;
                _unitOfWork.Complete();
            }

            return RedirectToAction("Index", "Home");
        }

        [HttpGet]
        public ActionResult Audit()
        {
            List<Requests> req = _unitOfWork.RequestsService.GetAllRequests();
            return View(req);
        }

        [HttpGet]
        public ActionResult AuditLog(int id)
        {
            var audit = _unitOfWork.AdminService.GetAuditLogById(id);
            return View(audit);
        }
    }
}

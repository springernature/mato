using ClosedXML.Excel;
using MATO.Classes;
using MATO.DataModel;
using MATO.NET.Helpers;
using MATO.NET.Persistence;
using MATO.NET.ViewModels;
using MATO.NET.ViewModels.v2Models;
using Microsoft.AspNet.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace MATO.NET.Controllers
{
    public class HomeController : Controller
    {
        private readonly IUnitOfWork _unitOfWork;

        public HomeController()
        {
            _unitOfWork = new UnitOfWork(new MATOContext());
        }

        public ActionResult Index()
        {
            UserHomeScreenViewModel model = new UserHomeScreenViewModel();
            var loggedIn = User.Identity.GetUserId();
            var role = _unitOfWork.UserService.GetUserRoleByUserId(loggedIn);
            model.User = _unitOfWork.UserService.GetUserById(loggedIn);
            var UserRequests = new List<Requests>();

            if (role == "SalesRep")
            {
                var userRequests = _unitOfWork.RequestsService.GetRequestsByUser(model.User);
                UserRequests = UserRequests.Concat(userRequests).Distinct().ToList();
            }

            if (role == "Manager")
            {
                var userRequests = _unitOfWork.RequestsService.GetRequestsByUser(model.User);
                UserRequests = UserRequests.Concat(userRequests).Distinct().ToList();

                var managerRequests = _unitOfWork.RequestsService.GetRequestsByRegion(model.User.Region.ToString());
                UserRequests = UserRequests.Concat(managerRequests).Distinct().ToList();
            }

            if (role == "Admin" || role == "SuperAdmin")
            {
                var adminRequests = _unitOfWork.RequestsService.GetAllRequests();
                UserRequests = UserRequests.Concat(adminRequests).Distinct().ToList();
            }

            if (role == "Author")
            {
                var authorRequests = _unitOfWork.RequestsService.GetRequestsForAuthor(model.User);
                UserRequests = UserRequests.Concat(authorRequests).Distinct().ToList();
            }

            model.UserRequests = UserRequests;

            return View(model);
        }

        [HttpPost]
        public JsonResult Approve(RequestApprovalViewModel model)
        {
            var loggedIn = User.Identity.GetUserId();
            var role = _unitOfWork.UserService.GetUserRoleByUserId(loggedIn);
            var user = _unitOfWork.UserService.GetUserById(loggedIn);
            var request = _unitOfWork.RequestsService.GetRequestById(model.RequestId);
            var whoSubmit = _unitOfWork.UserService.GetUserById(request.SubmitBy.Id);
            var region = _unitOfWork.AdminService.GetRegionById(request.Region.Id); //saqibh get region of the request not the user
            var manager = _unitOfWork.UserService.GetUserById(region.RegionalManagerId);
            var admin = _unitOfWork.UserService.GetSuperAdmin("SuperAdmin"); // Make non static // Get Admin Email. 
            var author = _unitOfWork.UserService.GetUserById(request.RequestedAuthor.Id);

            if (role == "Manager")
            {
                if (user.Region != region.Id)
                {
                    return Json("Wrong Region");
                }

                _unitOfWork.DecisionsService.ApproveRequest(model.RequestId);
                _unitOfWork.EmailService.SendMail(whoSubmit, 3, request);
                //saqibh 17/06/2020 superadmin needs to receive email
                _unitOfWork.EmailService.SendMail(admin, 17, request);
                _unitOfWork.Complete();
                return Json("Manager Approved");
            }
            else if (role == "Admin" || role == "SuperAdmin")
            {
                _unitOfWork.DecisionsService.AdminApproveRequest(model.RequestId);
                _unitOfWork.RequestsService.AddRequestToAuthorCalendar(request.RequestedAuthor.Id, request.EventOne.EventName, request.OutboundDate, request.InboundDate, request.Id);
                _unitOfWork.EmailService.SendMail(manager, 4, request); // Regional Manager
                _unitOfWork.EmailService.SendMail(whoSubmit, 6, request); // Sales Rep Who Submit
                _unitOfWork.EmailService.SendMail(author, 8, request); // Sales Rep Who Submit
                _unitOfWork.Complete();

                return Json("Admin Approved");
            }
            else if (role == "Author")
            {
                _unitOfWork.DecisionsService.AuthorApproveRequest(model.RequestId);
                _unitOfWork.EmailService.SendMail(admin, 9, request); // Super Admin
                _unitOfWork.EmailService.SendMail(manager, 10, request); // Regional Manager
                _unitOfWork.EmailService.SendMail(whoSubmit, 11, request); // Sales Rep Who Submit
                _unitOfWork.Complete();

                return Json("Author Approved");
            }
            else
            {
                //No permission
                return Json("No Permission");
            }
        }

        [HttpPost]
        public ActionResult Tentative(AuthorTentativeViewModel model)
        {
            var request = _unitOfWork.RequestsService.GetRequestById(model.RequestId);
            var whoSubmit = _unitOfWork.UserService.GetUserById(request.SubmitBy.Id);
            _unitOfWork.DecisionsService.ResponseWasTentative(model);
            _unitOfWork.EmailService.SendMail(whoSubmit, 16, request);

            return RedirectToAction("Index");
        }

        [HttpPost]
        public JsonResult Decline(RequestApprovalViewModel model) //this one
        {
            var loggedIn = User.Identity.GetUserId();
            var role = _unitOfWork.UserService.GetUserRoleByUserId(loggedIn);
            var user = _unitOfWork.UserService.GetUserById(loggedIn);
            var request = _unitOfWork.RequestsService.GetRequestById(model.RequestId);
            var whoSubmit = _unitOfWork.UserService.GetUserById(request.SubmitBy.Id);
            //var region = _unitOfWork.AdminService.GetRegionById(user.Region);
            var region = _unitOfWork.AdminService.GetRegionById(request.Region.Id); //saqibh get region of the request not the user
            var manager = _unitOfWork.UserService.GetUserById(region.RegionalManagerId);
            var admin = _unitOfWork.UserService.GetSuperAdmin("SuperAdmin"); // Make non static // Get Admin Email. 
            var author = _unitOfWork.UserService.GetUserById(request.RequestedAuthor.Id);

            if (role == "Manager")
            {
                if (user.Region != region.Id)
                {
                    return Json("Wrong Region");
                }

                _unitOfWork.DecisionsService.DeclineRequest(model.RequestId);
                _unitOfWork.EmailService.SendMail(whoSubmit, 2, request);
                //saqibh 17/06/2020 superadmin prob doesn't need to know if a request has been declined
                //_unitOfWork.EmailService.SendMail(admin, 18, request);
                _unitOfWork.Complete();
                return Json("Manager Declined");
            }
            else if (role == "SuperAdmin")
            {
                _unitOfWork.DecisionsService.DeclineRequest(model.RequestId);
                _unitOfWork.EmailService.SendMail(manager, 5, request); // Regional Manager
                _unitOfWork.EmailService.SendMail(whoSubmit, 7, request); // Sales Rep Who Submit
                _unitOfWork.Complete();

                return Json("Admin Declined");
            }
            else if (role == "Author")
            {
                _unitOfWork.DecisionsService.AuthorDeclineRequest(model.RequestId);
                _unitOfWork.EmailService.SendMail(admin, 12, request); // Super Admin
                _unitOfWork.EmailService.SendMail(manager, 13, request); // Regional Manager
                _unitOfWork.EmailService.SendMail(whoSubmit, 14, request); // Sales Rep Who Submit
                _unitOfWork.Complete();

                return Json("Author Declined");
            }
            else
            {
                //No permission
                return Json("No Permission");
            }
        }

        [HttpPost]
        public JsonResult ExportToExcel(int[] ids)
        {
            var loggedIn = User.Identity.Name;
            var user = _unitOfWork.UserService.GetUserByEmailAddress(loggedIn);
            var date = System.DateTime.Now.ToString("ddMMyy");
            var c1 = 2;
            var url = "";
            var file = "";
            var location = "";
            List<Requests> rev = new List<Requests>();

            foreach (var i in ids)
            {
                var r = _unitOfWork.RequestsService.GetRequestById(i);
                rev.Add(r);
            }

            using (var workbook = new XLWorkbook())
            {
                var worksheet = workbook.Worksheets.Add("AuthorData");
                //worksheet.Cell("A1").Value = "Hello World!";
                //worksheet.Cell("A2").FormulaA1 = "=MID(A1, 7, 5)";

                worksheet.Cell("A1").Value = "Request Id";
                worksheet.Cell("B1").Value = "Date Submit";
                worksheet.Cell("C1").Value = "Author Name";
                worksheet.Cell("D1").Value = "Event 1 Name";
                worksheet.Cell("E1").Value = "Event 1 Date";
                worksheet.Cell("F1").Value = "Event 1 City";
                worksheet.Cell("G1").Value = "Event 1 Type";
                worksheet.Cell("H1").Value = "Event 1 Target Sector";
                worksheet.Cell("I1").Value = "Event 1 Talk Type";
                worksheet.Cell("J1").Value = "Event 1 Expected Turnout";
                worksheet.Cell("K1").Value = "Event 2 Name";
                worksheet.Cell("L1").Value = "Event 2 Date";
                worksheet.Cell("M1").Value = "Event 2 City";
                worksheet.Cell("N1").Value = "Event 2 Type";
                worksheet.Cell("O1").Value = "Event 2 Target Sector";
                worksheet.Cell("P1").Value = "Event 2 Talk Type";
                worksheet.Cell("Q1").Value = "Event 2 Expected Turnout";
                worksheet.Cell("R1").Value = "Event 3 Name";
                worksheet.Cell("S1").Value = "Event 3 Date";
                worksheet.Cell("T1").Value = "Event 3 City";
                worksheet.Cell("U1").Value = "Event 3 Type";
                worksheet.Cell("V1").Value = "Event 3 Target Sector";
                worksheet.Cell("W1").Value = "Event 3 Talk Type";
                worksheet.Cell("X1").Value = "Event 3 Expected Turnout";
                worksheet.Cell("Y1").Value = "Requested By";
                worksheet.Cell("Z1").Value = "Region";
                worksheet.Cell("AA1").Value = "Title Promoted";
                worksheet.Cell("AB1").Value = "Manager Decision";
                worksheet.Cell("AC1").Value = "Administrator Decision";
                worksheet.Cell("AD1").Value = "Author Decision";

                foreach (var r in rev)
                {

                    worksheet.Cell("A" + c1).Value = r.Id;
                    worksheet.Cell("B" + c1).Value = r.SubmitDate;
                    worksheet.Cell("C" + c1).Value = r.RequestedAuthor.FullName;

                    worksheet.Cell("D" + c1).Value = r.EventOne.EventName;
                    worksheet.Cell("E" + c1).Value = r.EventOne.EventDate;
                    worksheet.Cell("F" + c1).Value = r.EventOne.EventCity;
                    worksheet.Cell("G" + c1).Value = r.EventOne.EventType.Name;
                    worksheet.Cell("H" + c1).Value = r.EventOne.TargetSector.Name;
                    worksheet.Cell("I" + c1).Value = r.EventOne.TalkType.Name;
                    worksheet.Cell("J" + c1).Value = r.EventOne.ExpectedTurnout;

                    if (r.EventTwo != null)
                    {
                        worksheet.Cell("K" + c1).Value = r.EventTwo.EventName;
                        worksheet.Cell("L" + c1).Value = r.EventTwo.EventDate;
                        worksheet.Cell("M" + c1).Value = r.EventTwo.EventCity;
                        worksheet.Cell("N" + c1).Value = r.EventTwo.EventType.Name;
                        worksheet.Cell("O" + c1).Value = r.EventTwo.TargetSector.Name;
                        worksheet.Cell("P" + c1).Value = r.EventTwo.TalkType.Name;
                        worksheet.Cell("Q" + c1).Value = r.EventTwo.ExpectedTurnout;
                    }
                    else
                    {
                        worksheet.Cell("K" + c1).Value = "";
                        worksheet.Cell("L" + c1).Value = "";
                        worksheet.Cell("M" + c1).Value = "";
                        worksheet.Cell("N" + c1).Value = "";
                        worksheet.Cell("O" + c1).Value = "";
                        worksheet.Cell("P" + c1).Value = "";
                        worksheet.Cell("Q" + c1).Value = "";
                    }

                    if (r.EventThree != null)
                    {
                        worksheet.Cell("R" + c1).Value = r.EventThree.EventName;
                        worksheet.Cell("S" + c1).Value = r.EventThree.EventDate;
                        worksheet.Cell("T" + c1).Value = r.EventThree.EventCity;
                        worksheet.Cell("U" + c1).Value = r.EventThree.EventType.Name;
                        worksheet.Cell("V" + c1).Value = r.EventThree.TargetSector.Name;
                        worksheet.Cell("W" + c1).Value = r.EventThree.TalkType.Name;
                        worksheet.Cell("X" + c1).Value = r.EventThree.ExpectedTurnout;
                    }
                    else
                    {
                        worksheet.Cell("R" + c1).Value = "";
                        worksheet.Cell("S" + c1).Value = "";
                        worksheet.Cell("T" + c1).Value = "";
                        worksheet.Cell("U" + c1).Value = "";
                        worksheet.Cell("V" + c1).Value = "";
                        worksheet.Cell("W" + c1).Value = "";
                        worksheet.Cell("X" + c1).Value = "";
                    }

                    worksheet.Cell("Y" + c1).Value = r.SubmitBy.FullName;
                    worksheet.Cell("Z" + c1).Value = r.Region.Name;
                    worksheet.Cell("AA" + c1).Value = r.PromotedTitle.Name;

                    if (r.ManagerApproval == true)
                    {
                        worksheet.Cell("AB" + c1).Value = "Approved";
                    }
                    else if (r.ManagerApproval == false && r.Declined == true)
                    {
                        worksheet.Cell("AB" + c1).Value = "Declined";
                    }
                    else
                    {
                        worksheet.Cell("AB" + c1).Value = "Pending";
                    }

                    if (r.AdminApproval == true)
                    {
                        worksheet.Cell("AC" + c1).Value = "Approved";
                    }
                    else if (r.AdminApproval == false && r.Declined == true && r.ManagerApproval == true)
                    {
                        worksheet.Cell("AC" + c1).Value = "Declined";
                    }
                    else
                    {
                        worksheet.Cell("AC" + c1).Value = "Pending";
                    }

                    if (r.Finalised == true)
                    {
                        worksheet.Cell("AD" + c1).Value = "Approved";
                    }
                    else if (r.Finalised == false && r.Declined == true && r.ManagerApproval == true && r.AdminApproval == true)
                    {
                        worksheet.Cell("AD" + c1).Value = "Declined";
                    }
                    else
                    {
                        worksheet.Cell("AD" + c1).Value = "Pending";
                    }

                    c1++;
                }

                location = AppDomain.CurrentDomain.BaseDirectory + @"Reporting\Reports\MATO_" + user.FirstName + "_" + user.LastName + "_" + date + ".xlsx";
                workbook.SaveAs(location);
                url = "http://" + Request.Url.Authority + "/Reporting/Reports/MATO_" + user.FirstName + "_" + user.LastName + "_" + date + ".xlsx";

                return Json(url);
            }
        }

        [HttpPost]
        public JsonResult ExportDateRange(DateRange dr)
        {
            var loggedIn = User.Identity.Name;
            var user = _unitOfWork.UserService.GetUserByEmailAddress(loggedIn);
            var role = _unitOfWork.UserService.GetUserRoleByUserId(user.Id);

            var fd = Convert.ToDateTime(dr.fromDate);
            var td = Convert.ToDateTime(dr.toDate);

            var c1 = 2;
            var url = "";
            var file = "";
            var location = "";
            List<Requests> rev = new List<Requests>();

            if(role == "Author")
            {
                return Json("Author cannot export");
            }
            else if (role == "Manager")
            {
                rev = _unitOfWork.RequestsService.GetReviewsByDateRange(fd, td, user.Region);
            }
            else if(role == "SalesRep")
            {
                rev = _unitOfWork.RequestsService.GetReviewsByDateRangeForRep(fd, td, user);
            }
            else
            {
                rev = _unitOfWork.RequestsService.GetReviewsByDateRange(fd, td, 0);
            }

            if (rev.Count == 0)
            {
                return Json("Empty Date Range");
            }

            using (var workbook = new XLWorkbook())
            {
                var worksheet = workbook.Worksheets.Add("AuthorData");
                //worksheet.Cell("A1").Value = "Hello World!";
                //worksheet.Cell("A2").FormulaA1 = "=MID(A1, 7, 5)";

                worksheet.Cell("A1").Value = "Request Id";
                worksheet.Cell("B1").Value = "Date Submit";
                worksheet.Cell("C1").Value = "Author Name";
                worksheet.Cell("D1").Value = "Event 1 Name";
                worksheet.Cell("E1").Value = "Event 1 Date";
                worksheet.Cell("F1").Value = "Event 1 City";
                worksheet.Cell("G1").Value = "Event 1 Type";
                worksheet.Cell("H1").Value = "Event 1 Target Sector";
                worksheet.Cell("I1").Value = "Event 1 Talk Type";
                worksheet.Cell("J1").Value = "Event 1 Expected Turnout";
                worksheet.Cell("K1").Value = "Event 2 Name";
                worksheet.Cell("L1").Value = "Event 2 Date";
                worksheet.Cell("M1").Value = "Event 2 City";
                worksheet.Cell("N1").Value = "Event 2 Type";
                worksheet.Cell("O1").Value = "Event 2 Target Sector";
                worksheet.Cell("P1").Value = "Event 2 Talk Type";
                worksheet.Cell("Q1").Value = "Event 2 Expected Turnout";
                worksheet.Cell("R1").Value = "Event 3 Name";
                worksheet.Cell("S1").Value = "Event 3 Date";
                worksheet.Cell("T1").Value = "Event 3 City";
                worksheet.Cell("U1").Value = "Event 3 Type";
                worksheet.Cell("V1").Value = "Event 3 Target Sector";
                worksheet.Cell("W1").Value = "Event 3 Talk Type";
                worksheet.Cell("X1").Value = "Event 3 Expected Turnout";
                worksheet.Cell("Y1").Value = "Requested By";
                worksheet.Cell("Z1").Value = "Region";
                worksheet.Cell("AA1").Value = "Title Promoted";
                worksheet.Cell("AB1").Value = "Manager Decision";
                worksheet.Cell("AC1").Value = "Administrator Decision";
                worksheet.Cell("AD1").Value = "Author Decision";

                foreach (var r in rev)
                {

                    worksheet.Cell("A" + c1).Value = r.Id;
                    worksheet.Cell("B" + c1).Value = r.SubmitDate;
                    worksheet.Cell("C" + c1).Value = r.RequestedAuthor.FullName;

                    worksheet.Cell("D" + c1).Value = r.EventOne.EventName;
                    worksheet.Cell("E" + c1).Value = r.EventOne.EventDate;
                    worksheet.Cell("F" + c1).Value = r.EventOne.EventCity;
                    worksheet.Cell("G" + c1).Value = r.EventOne.EventType.Name;
                    worksheet.Cell("H" + c1).Value = r.EventOne.TargetSector.Name;
                    worksheet.Cell("I" + c1).Value = r.EventOne.TalkType.Name;
                    worksheet.Cell("J" + c1).Value = r.EventOne.ExpectedTurnout;

                    if (r.EventTwo != null)
                    {
                        worksheet.Cell("K" + c1).Value = r.EventTwo.EventName;
                        worksheet.Cell("L" + c1).Value = r.EventTwo.EventDate;
                        worksheet.Cell("M" + c1).Value = r.EventTwo.EventCity;
                        worksheet.Cell("N" + c1).Value = r.EventTwo.EventType.Name;
                        worksheet.Cell("O" + c1).Value = r.EventTwo.TargetSector.Name;
                        worksheet.Cell("P" + c1).Value = r.EventTwo.TalkType.Name;
                        worksheet.Cell("Q" + c1).Value = r.EventTwo.ExpectedTurnout;
                    }
                    else
                    {
                        worksheet.Cell("K" + c1).Value = "";
                        worksheet.Cell("L" + c1).Value = "";
                        worksheet.Cell("M" + c1).Value = "";
                        worksheet.Cell("N" + c1).Value = "";
                        worksheet.Cell("O" + c1).Value = "";
                        worksheet.Cell("P" + c1).Value = "";
                        worksheet.Cell("Q" + c1).Value = "";
                    }

                    if (r.EventThree != null)
                    {
                        worksheet.Cell("R" + c1).Value = r.EventThree.EventName;
                        worksheet.Cell("S" + c1).Value = r.EventThree.EventDate;
                        worksheet.Cell("T" + c1).Value = r.EventThree.EventCity;
                        worksheet.Cell("U" + c1).Value = r.EventThree.EventType.Name;
                        worksheet.Cell("V" + c1).Value = r.EventThree.TargetSector.Name;
                        worksheet.Cell("W" + c1).Value = r.EventThree.TalkType.Name;
                        worksheet.Cell("X" + c1).Value = r.EventThree.ExpectedTurnout;
                    }
                    else
                    {
                        worksheet.Cell("R" + c1).Value = "";
                        worksheet.Cell("S" + c1).Value = "";
                        worksheet.Cell("T" + c1).Value = "";
                        worksheet.Cell("U" + c1).Value = "";
                        worksheet.Cell("V" + c1).Value = "";
                        worksheet.Cell("W" + c1).Value = "";
                        worksheet.Cell("X" + c1).Value = "";
                    }

                    worksheet.Cell("Y" + c1).Value = r.SubmitBy.FullName;
                    worksheet.Cell("Z" + c1).Value = r.Region.Name;
                    worksheet.Cell("AA" + c1).Value = r.PromotedTitle.Name;

                    if (r.ManagerApproval == true)
                    {
                        worksheet.Cell("AB" + c1).Value = "Approved";
                    }
                    else if (r.ManagerApproval == false && r.Declined == true)
                    {
                        worksheet.Cell("AB" + c1).Value = "Declined";
                    }
                    else
                    {
                        worksheet.Cell("AB" + c1).Value = "Pending";
                    }

                    if (r.AdminApproval == true)
                    {
                        worksheet.Cell("AC" + c1).Value = "Approved";
                    }
                    else if (r.AdminApproval == false && r.Declined == true && r.ManagerApproval == true)
                    {
                        worksheet.Cell("AC" + c1).Value = "Declined";
                    }
                    else
                    {
                        worksheet.Cell("AC" + c1).Value = "Pending";
                    }

                    if (r.Finalised == true)
                    {
                        worksheet.Cell("AD" + c1).Value = "Approved";
                    }
                    else if (r.Finalised == false && r.Declined == true && r.ManagerApproval == true && r.AdminApproval == true)
                    {
                        worksheet.Cell("AD" + c1).Value = "Declined";
                    }
                    else
                    {
                        worksheet.Cell("AD" + c1).Value = "Pending";
                    }

                    c1++;
                }

                var fileNamefd = dr.fromDate.ToString().Replace("/", "");
                var fileNametd = dr.toDate.ToString().Replace("/", "");

                location = AppDomain.CurrentDomain.BaseDirectory + @"Reporting\Reports\MATO_" + fileNamefd + "_to_" + fileNametd + "_" + user.FirstName + user.LastName + ".xlsx";
                workbook.SaveAs(location);
                url = "http://" + Request.Url.Authority + "/Reporting/Reports/MATO_" + fileNamefd + "_to_" + fileNametd + "_" + user.FirstName + user.LastName + ".xlsx";

                return Json(url);
            }
        }
    }
}
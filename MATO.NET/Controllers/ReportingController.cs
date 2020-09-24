using System;
using System.Collections.Generic;
using System.Web.Mvc;
using ClosedXML.Excel;
using MATO.Classes;
using MATO.DataModel;
using MATO.NET.Persistence;
using MATO.NET.ViewModels;
using Microsoft.AspNet.Identity;
using Newtonsoft.Json;

namespace MATO.NET.Controllers
{
    public class ReportingController : Controller
    {
        private readonly IUnitOfWork _unitOfWork;

        public ReportingController()
        {
            _unitOfWork = new UnitOfWork(new MATOContext());
        }

        // GET: Reporting
        public ActionResult Index()
        {
            return View();
        }

        [HttpGet]
        public ActionResult Requests()
        {
            ReportingRequestsViewModel model = new ReportingRequestsViewModel();
            var loggedIn = User.Identity.GetUserId();
            var user = _unitOfWork.UserService.GetUserById(loggedIn);
            var role = _unitOfWork.UserService.GetUserRoleByUserId(loggedIn);

            if (role == "Manager")
            {
                model.Requests = _unitOfWork.DecisionsService.GetReportingRequestsForManager(loggedIn);
            }
            else if (role == "Admin" || role == "SuperAdmin")
            {
                model.Requests = _unitOfWork.DecisionsService.GetReportingRequestsForAdmins(loggedIn);
            }

            return View(model);
        }

        [HttpGet]
        public ActionResult Authors()
        {
            AuthorsViewModel model =
                new AuthorsViewModel { Authors = _unitOfWork.UserService.GetApplicationUsersInRole("Author") };
            return View(model);
        }

        [HttpGet]
        public ActionResult Yearly()
        {
            return View();
        }

        [HttpGet]
        public ActionResult Title()
        {
            TitlesViewModel model =
                new TitlesViewModel { Titles = _unitOfWork.RequestsService.GetPromotedTitleList() };
            return View(model);
        }

        [HttpGet]
        public ActionResult Region()
        {
            var loggedIn = User.Identity.GetUserId();
            var region = _unitOfWork.AdminService.GetRegionsByManagerId(loggedIn);
            var role = _unitOfWork.UserService.GetUserRoleByUserId(loggedIn);

            RegionsViewModel model = new RegionsViewModel();
            if (role == "Manager")
            {
                model.Regions = region;
            }
            else if (role == "Admin" || role == "SuperAdmin")
            {
                model.Regions = _unitOfWork.AdminService.GetRegions();
            }

            return View(model);
        }

        [HttpGet]
        public ActionResult EventType()
        {
            EventTypeViewModel model =
                new EventTypeViewModel { EventTypes = _unitOfWork.RequestsService.GetEventTypes() };
            return View(model);
        }

        //[HttpPost]
        //public JsonResult GetAuthorRequests(string id)
        //{
        //    var author = _unitOfWork.UserService.GetUserById(id);
        //    var requests = _unitOfWork.RequestsService.GetRequestsForAuthor(author);
        //    var json = JsonConvert.SerializeObject(requests, new Newtonsoft.Json.Converters.StringEnumConverter());

        //    return Json(json);
        //}

        [HttpPost]
        public JsonResult ExportToExcel(string id, string type)
        {
            var requests = new List<Requests>();
            var author = new AppUser();
            var reportType = "";
            var title = new PromotedTitle();
            var region = new Region();
            var eventType = new EventType();

            if (type == "Author")
            {
                author = _unitOfWork.UserService.GetUserByEmailAddress(id);
                requests = _unitOfWork.RequestsService.GetRequestsForAuthor(author);
                reportType = "Author";
            }
            else if (type == "Year")
            {
                requests = _unitOfWork.RequestsService.GetRequestsByYear(id);
                reportType = "Year";
            }
            else if (type == "Title")
            {
                requests = _unitOfWork.RequestsService.GetRequestsByTitle(id);
                var titleValue = Int32.Parse(id);
                title = _unitOfWork.RequestsService.GetPromotedTitleById(titleValue);
                reportType = "Title";
            }
            else if (type == "Region")
            {
                requests = _unitOfWork.RequestsService.GetRequestsByRegion(id);
                var regionValue = Int32.Parse(id);
                region = _unitOfWork.RequestsService.GetRegionById(regionValue);
                reportType = "Region";
            }
            else if (type == "Event")
            {
                requests = _unitOfWork.RequestsService.GetRequestsByEventType(id);
                var eventValue = Int32.Parse(id);
                eventType = _unitOfWork.RequestsService.GetEventTypeById(eventValue);
                reportType = "EventType";
            }


            var date = System.DateTime.Now.ToString("ddMMyy");
            var location = "";
            var url = "";
            var c1 = 2;

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

                foreach (var r in requests)
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

                if (reportType == "Author")
                {
                    location = AppDomain.CurrentDomain.BaseDirectory + @"Reporting\Reports\Author_" + author.FullName + "_" + date + ".xlsx";
                    workbook.SaveAs(location);
                    url = "http://" + Request.Url.Authority + "/Reporting/Reports/Author_" + author.FullName + "_" + date + ".xlsx";
                }
                else if (reportType == "Year")
                {
                    location = AppDomain.CurrentDomain.BaseDirectory + @"Reporting\Reports\Year_" + id + ".xlsx";
                    workbook.SaveAs(location);
                    url = "http://" + Request.Url.Authority + "/Reporting/Reports/Year_" + id + ".xlsx";
                }
                else if (reportType == "Title")
                {
                    location = AppDomain.CurrentDomain.BaseDirectory + @"Reporting\Reports\Title_" + title.Name + ".xlsx";
                    workbook.SaveAs(location);
                    url = "http://" + Request.Url.Authority + "/Reporting/Reports/Title_" + title.Name + ".xlsx";
                }
                else if (reportType == "Region")
                {
                    location = AppDomain.CurrentDomain.BaseDirectory + @"Reporting\Reports\Region_" + region.Name + ".xlsx";
                    workbook.SaveAs(location);
                    url = "http://" + Request.Url.Authority + "/Reporting/Reports/Region_" + region.Name + ".xlsx";
                }
                else if (reportType == "EventType")
                {
                    location = AppDomain.CurrentDomain.BaseDirectory + @"Reporting\Reports\EventType_" + eventType.Name + ".xlsx";
                    workbook.SaveAs(location);
                    url = "http://" + Request.Url.Authority + "/Reporting/Reports/EventType_" + eventType.Name + ".xlsx";
                }
            }

            return Json(url);
        }
    }
}
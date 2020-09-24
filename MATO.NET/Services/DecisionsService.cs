using System;
using System.Collections.Generic;
using System.Linq;
using MATO.DataModel;
using MATO.Classes;
using MATO.NET.Interfaces;
using System.Data.Entity;
using System.Web.Script.Serialization;
using MATO.DataModel.Migrations;
using MATO.NET.ViewModels.v2Models;

namespace MATO.NET.Services
{
    public class DecisionsService : IDecisionsService
    {
        private readonly MATOContext context;

        public DecisionsService(MATOContext _context)
        {
            context = _context;
        }

        public Region IsLoggedInUserRegionManager(string id)
        {
            return context.Region.FirstOrDefault(r => r.RegionalManagerId == id);
        }

        public List<Requests> GetRequestsByRegion(Region region)
        {
            return context.Requests.Include(e => e.EventOne.TalkType)
                .Include(e => e.EventTwo.TalkType)
                .Include(e => e.EventThree.TalkType)
                .Include("RequestedAuthor")
                .Include(e => e.EventOne.EventType)
                .Include(e => e.EventTwo.EventType)
                .Include(e => e.EventThree.EventType)
                .Include(e => e.EventOne.TargetSector)
                .Include(e => e.EventTwo.TargetSector)
                .Include(e => e.EventThree.TargetSector)
                .Where(r => r.Region.Id == region.Id && r.Cancelled == false && r.Finalised == false && r.ManagerApproval == false && r.Declined == false).ToList();
        }

        public List<Requests> GetRequestsForAdministrator()
        {
            return context.Requests.Include(e => e.EventOne.TalkType)
                .Include(e => e.EventTwo.TalkType)
                .Include(e => e.EventThree.TalkType)
                .Include("RequestedAuthor")
                .Include(e => e.EventOne.EventType)
                .Include(e => e.EventTwo.EventType)
                .Include(e => e.EventThree.EventType)
                .Include(e => e.EventOne.TargetSector)
                .Include(e => e.EventTwo.TargetSector)
                .Include(e => e.EventThree.TargetSector)
                .Where(r => r.Cancelled == false && r.Finalised == false && r.ManagerApproval && r.AdminApproval == false && r.Declined == false).ToList();
        }

        public List<Requests> GetReportingRequestsForManager(string loggedIn)
        {
            return context.Requests.Include(e => e.EventOne.TalkType)
                .Include(e => e.EventTwo.TalkType)
                .Include(e => e.EventThree.TalkType)
                .Include("RequestedAuthor")
                .Include(e => e.EventOne.EventType)
                .Include(e => e.EventTwo.EventType)
                .Include(e => e.EventThree.EventType)
                .Include(e => e.EventOne.TargetSector)
                .Include(e => e.EventTwo.TargetSector)
                .Include(e => e.EventThree.TargetSector)
                .Where(r => r.Region.RegionalManagerId == loggedIn).ToList();
        }

        public List<Requests> GetReportingRequestsForAdmins(string loggedIn)
        {
            return context.Requests.Include(e => e.EventOne.TalkType)
                .Include(e => e.EventTwo.TalkType)
                .Include(e => e.EventThree.TalkType)
                .Include("RequestedAuthor")
                .Include(e => e.EventOne.EventType)
                .Include(e => e.EventTwo.EventType)
                .Include(e => e.EventThree.EventType)
                .Include(e => e.EventOne.TargetSector)
                .Include(e => e.EventTwo.TargetSector)
                .Include(e => e.EventThree.TargetSector)
                .ToList();
        }

        public List<Requests> GetRequestsForAuthor(string loggedInId)
        {
            return context.Requests.Include(e => e.EventOne.TalkType)
                .Include(e => e.EventTwo.TalkType)
                .Include(e => e.EventThree.TalkType)
                .Include("RequestedAuthor")
                .Include(e => e.EventOne.EventType)
                .Include(e => e.EventTwo.EventType)
                .Include(e => e.EventThree.EventType)
                .Include(e => e.EventOne.TargetSector)
                .Include(e => e.EventTwo.TargetSector)
                .Include(e => e.EventThree.TargetSector)
                .Where(e => e.ManagerApproval && e.AdminApproval && e.Finalised == false && e.RequestedAuthor.Id == loggedInId).ToList();
        }

        public Requests ApproveRequest(int id)
        {
            var request = context.Requests.FirstOrDefault(r => r.Id == id);
            request.ManagerApproval = true;
            request.ManagerApprovalDate = DateTime.Now;
            return request;
        }

        public Requests DeclineRequest(int id)
        {
            var request = context.Requests
                .Include("RequestedAuthor")
                .Include("EventOne")
                .FirstOrDefault(r => r.Id == id);
            var calendar = context.Calendar.FirstOrDefault(c => c.Author.Id == request.RequestedAuthor.Id);
            var json = "[]";

            if (calendar != null)
            {
                JavaScriptSerializer jsSerializer = new JavaScriptSerializer();
                var cevents = jsSerializer.Deserialize<List<CalendarEvent>>(calendar.Data);

                foreach (var c in cevents.ToList())
                {
                    if (c.title == request.EventOne.EventName)
                    {
                        cevents.Remove(c);
                    }
                }

                json = new JavaScriptSerializer().Serialize(cevents);
            }

            else
            {
                calendar = new Calendar();
            }

            calendar.Data = json;
            calendar.Author = request.RequestedAuthor;

            request.Declined = true;
            request.DeclinedDate = DateTime.Now;
            request.ManagerApprovalDate = DateTime.Now;
            request.AdminApprovalDate = DateTime.Now; //saqibh added to distinguish between manager decline....don't judge me!!!
            context.SaveChanges();
            return request;
        }

        public Requests AdminApproveRequest(int id)
        {
            var request = context.Requests.FirstOrDefault(r => r.Id == id);
            request.AdminApproval = true;
            request.AdminApprovalDate = DateTime.Now;
            return request;
        }

        public Requests AdminDeclineRequest(int id)
        {
            var request = context.Requests.Include("RequestedAuthor").Include("EventOne").FirstOrDefault(r => r.Id == id);
            var calendar = context.Calendar.FirstOrDefault(c => c.Author.Id == request.RequestedAuthor.Id);
            var json = "[]";

            if (calendar != null)
            {
                JavaScriptSerializer jsSerializer = new JavaScriptSerializer();
                var cevents = jsSerializer.Deserialize<List<CalendarEvent>>(calendar.Data);

                foreach (var c in cevents.ToList())
                {
                    if (c.title == request.EventOne.EventName)
                    {
                        cevents.Remove(c);
                    }
                }

                json = new JavaScriptSerializer().Serialize(cevents);

            }
            else
            {
                calendar = new Calendar();
            }

            calendar.Data = json;
            calendar.Author = request.RequestedAuthor;

            request.Declined = true;
            request.DeclinedDate = DateTime.Now;
            request.AdminApprovalDate = DateTime.Now;
            context.SaveChanges();
            return request;
        }

        public Requests AuthorApproveRequest(int id)
        {
            var request = context.Requests.FirstOrDefault(r => r.Id == id);
            request.Finalised = true;
            request.FinalisedDate = DateTime.Now;
            return request;
        }

        public Requests AuthorDeclineRequest(int id)
        {
            var request = context.Requests.Include("RequestedAuthor").Include("EventOne").FirstOrDefault(r => r.Id == id);
            var calendar = context.Calendar.FirstOrDefault(c => c.Author.Id == request.RequestedAuthor.Id);
            var json = "[]";

            if (calendar != null)
            {
                JavaScriptSerializer jsSerializer = new JavaScriptSerializer();
                var cevents = jsSerializer.Deserialize<List<CalendarEvent>>(calendar.Data);

                foreach (var c in cevents.ToList())
                {
                    if (c.title == request.EventOne.EventName)
                    {
                        cevents.Remove(c);
                    }
                }

                json = new JavaScriptSerializer().Serialize(cevents);

            }
            else
            {
                calendar = new Calendar();
            }
            calendar.Data = json;
            calendar.Author = request.RequestedAuthor;

            request.Declined = true;
            request.DeclinedDate = DateTime.Now;
            request.FinalisedDate = DateTime.Now;
            context.SaveChanges();
            return request;
        }

        public Requests ResponseWasTentative(AuthorTentativeViewModel model)
        {
            var request = context.Requests.FirstOrDefault(x => x.Id == model.RequestId);
            request.AdminApproval = false;
            request.AdminApprovalDate = null;
            request.TentativeReason = model.TentativeReason;
            context.SaveChanges();

            return request;
        }

    }
}
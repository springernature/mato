using MATO.Classes;
using MATO.DataModel;
using MATO.NET.Interfaces;
using MATO.NET.ViewModels;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Web;
using System.Web.Script.Serialization;

namespace MATO.NET.Services
{
    public class RequestsService : IRequestsService
    {
        private readonly MATOContext context;

        public RequestsService(MATOContext _context)
        {
            context = _context;
        }

        public AppUser GetAppUserFromId(string id)
        {
            return context.Users.FirstOrDefault(u => u.Id == id);
        }

        public Requests CreateRequest(SubmitRequestViewModel model, AppUser submitBy)
        {
            Requests req = new Requests
            {
                SubmitBy = GetAppUserFromId(submitBy.Id),
                SubmitDate = DateTime.Now,
                RequestedAuthor = GetAppUserFromId(model.SelectedAuthorId),
                PromotedTitle = GetPromotedTitleById(model.TitlePromotedId),
                TitleForecast = CreateTitleForecast(model.Forecast, model.TitlePromotedId),
                Region = context.Region.FirstOrDefault(a => a.Id == submitBy.Region),
                Outbound = model.Outbound,
                Inbound = model.Inbound,
                OutboundDate = model.OutboundDate ?? DateTime.Now,
                InboundDate = model.InboundDate ?? DateTime.Now,
                Declined = false,
                ManagerApproval = false,
                AuthorNotesBySalesRep = model.AuthorNotesBySalesRep,
                NonAuthorNotesBySalesRep = model.NonAuthorNotesBySalesRep,
                SessionDescription=model.SessionDescription,
               LocalAuthorContact=model.LocalAuthorContact
            };
            context.Requests.Add(req);
            context.SaveChanges();
            return req;
        }

        public Event CreateEvent(EventViewModel evm, Requests req)
        {
            Event ev = new Event
            {
                EventDate = evm.EventDate ?? DateTime.Now,
                EventName = evm.EventName,
                EventCity = evm.EventCity,
                EventType = GetEventTypeById(evm.SelectedEventId),
                TargetSector = GetTargetSectorById(evm.SectorTargetedId),
                TalkType = GetTalkTypeById(evm.TalkTypeId),
                ExpectedTurnout = evm.ExpectedTurnout

            };


            context.Event.Add(ev);
            return ev;
        }

        public Requests UpdateRequestWithEvent(Requests request, Event Event, int eventNumber)
        {
            var req = context.Requests.FirstOrDefault(r => r.Id == request.Id);
            switch (eventNumber)
            {
                case 1:
                    if (req != null) req.EventOne = Event;
                    break;
                case 2:
                    if (req != null) req.EventTwo = Event;
                    break;
                case 3:
                    if (req != null) req.EventThree = Event;
                    break;
            }

            return request;
        }

        public Requests GetRequestById(int id)
        {
            return context.Requests
                .Include("EventOne")
                .Include("EventTwo")
                .Include("EventThree")
                .Include(e => e.EventOne.TalkType)
                .Include(e => e.EventTwo.TalkType)
                .Include(e => e.EventThree.TalkType)
                .Include("RequestedAuthor")
                .Include(e => e.EventOne.EventType)
                .Include(e => e.EventTwo.EventType)
                .Include(e => e.EventThree.EventType)
                .Include(e => e.EventOne.TargetSector)
                .Include(e => e.EventTwo.TargetSector)
                .Include(e => e.EventThree.TargetSector)
                .Include(e => e.PromotedTitle)
                .Include(e => e.Region)
                .Include(e => e.TitleForecast)
                .Include(e => e.SubmitBy)
                .FirstOrDefault(r => r.Id == id);
        }

        public TitleForecast CreateTitleForecast(TitleForecast forecast, int promo)
        {
            TitleForecast tf = new TitleForecast
            {
                Year1 = forecast.Year1,
                Year2 = forecast.Year2,
                Year3 = forecast.Year3,
                PromotedTitle = GetPromotedTitleById(promo),
            };

            context.TitleForecast.Add(tf);
            return tf;
        }

        public EventType GetEventTypeById(int id)
        {
            return context.EventTypes.FirstOrDefault(e => e.Id == id);
        }

        public TargetSector GetTargetSectorById(int id)
        {
            return context.TargetSector.FirstOrDefault(e => e.Id == id);
        }

        public PromotedTitle GetPromotedTitleById(int id)
        {
            return context.PromotedTitle.FirstOrDefault(e => e.Id == id);
        }

        public TalkType GetTalkTypeById(int id)
        {
            return context.TalkType.FirstOrDefault(e => e.Id == id);
        }

        public List<Requests> GetRequestsByUser(AppUser user)
        {
            return context.Requests.Include("EventOne")
                                   .Include("EventOne.TalkType")
                                   .Include("EventOne.EventType")
                                   .Include("EventOne.TargetSector")
                                   .Include("EventTwo")
                                   .Include("EventTwo.TalkType")
                                   .Include("EventTwo.EventType")
                                   .Include("EventTwo.TargetSector")
                                   .Include("EventThree")
                                   .Include("EventThree.TalkType")
                                   .Include("EventThree.EventType")
                                   .Include("EventThree.TargetSector")
                                   .Include("RequestedAuthor")
                                   .Include("PromotedTitle")
                                   .Include("TitleForecast")
                                   .Include("Region")
                                   .Where(r => r.SubmitBy.Id == user.Id).ToList();
        }

        public List<Requests> GetFinalisedRequests(string user)
        {
            return context.Requests.Include("EventOne")
                                   .Include("EventOne.TalkType")
                                   .Include("EventOne.EventType")
                                   .Include("EventOne.TargetSector")
                                   .Include("EventTwo")
                                   .Include("EventTwo.TalkType")
                                   .Include("EventTwo.EventType")
                                   .Include("EventTwo.TargetSector")
                                   .Include("EventThree")
                                   .Include("EventThree.TalkType")
                                   .Include("EventThree.EventType")
                                   .Include("EventThree.TargetSector")
                                   .Include("RequestedAuthor")
                                   .Include("PromotedTitle")
                                   .Include("TitleForecast")
                                   .Where(r => r.Finalised == true && r.SubmitBy.Id == user && r.Declined == false).ToList();
        }

        public List<Requests> GetCancelledRequests(string user)
        {
            return context.Requests.Include("EventOne").Include("EventTwo").Include("EventThree").Include("RequestedAuthor").Include("PromotedTitle").Include("TitleForecast").Where(r => r.Cancelled && r.SubmitBy.Id == user).ToList();
        }

        public List<Requests> GetDeclinedRequests(string user)
        {
            return context.Requests.Include("EventOne").Include("EventTwo").Include("EventThree").Include("RequestedAuthor").Include("PromotedTitle").Include("TitleForecast").Where(r => r.Declined && r.SubmitBy.Id == user).ToList();
        }

        public List<Requests> GetInCompleteRequests(string user)
        {
            return context.Requests.Include("EventOne").Include("EventTwo").Include("EventThree").Include("RequestedAuthor").Include("PromotedTitle").Include("TitleForecast").Where(r => r.Complete == false && r.SubmitBy.Id == user).ToList();
        }

        public List<Requests> GetPendingRequests(string user)
        {
            return context.Requests.Include("EventOne").Include("EventTwo").Include("EventThree").Include("RequestedAuthor").Include("PromotedTitle").Include("TitleForecast").Where(r => r.Cancelled == false && r.Finalised == false && r.Complete == false && r.SubmitBy.Id == user && r.Declined == false).ToList();
        }

        public List<EventType> GetEventTypes()
        {
            return context.EventTypes.Where(e => e.Active).OrderBy(e => e.Name).ToList();
        }

        public AppUser GetAuthorDetail(string id)
        {
            return context.Users.FirstOrDefault(u => u.Id == id);
        }

        public List<Country> GetCountryList()
        {
            return context.Countries.OrderBy(e => e.Name).ToList();
        }

        public List<PromotedTitle> GetPromotedTitleList()
        {
            return context.PromotedTitle.OrderBy(e => e.Name).ToList();
        }
        public List<PromotedTitle> GetPromotedTitleWithTargetSectors()
        {
            var title = from tsk in context.PromotedTitle.Include(t => t.TargetSector).OrderBy(e => e.Name)
                        select tsk;
            return title.ToList();
        }
        public List<TargetSector> GetSectorsTargeted()
        {
            return context.TargetSector.Where(e => e.Active).OrderBy(e => e.Name).ToList();
        }

        public List<TalkType> GetTalkTypes()
        {
            return context.TalkType.Where(e => e.Active).OrderBy(e => e.Name).ToList();
        }

        public TitleForecast GetTitleForecastById(int id)
        {
            return context.TitleForecast.FirstOrDefault(f => f.Id == id);
        }

        public Region GetRegionById(int id)
        {
            return context.Region.FirstOrDefault(r => r.Id == id);
        }

        public Calendar AddRequestToAuthorCalendar(string id, string eventName, DateTime? startDate, DateTime? endDate, int reqId, DateTime eventDate)
        {
            var author = context.Users.FirstOrDefault(u => u.Id == id);
            var calendar = context.Calendar.FirstOrDefault(c => c.Author.Id == author.Id);
            var insertCalendar = false;

            if (calendar == null)
            {
                Calendar newCal = new Calendar { Data = "[]" };
                calendar = newCal;
                insertCalendar = true;
            }

            JavaScriptSerializer jsSerializer = new JavaScriptSerializer();
            var cevents = jsSerializer.Deserialize<List<CalendarEvent>>(calendar.Data);

            //var cal = new CalendarEvent
            //{
            //    title = eventName,
            //    start = startDate != null ? startDate.Value.ToString("yyyy-MM-dd") : DateTime.Now.ToString("yyyy-MM-dd"),
            //    end = endDate != null ? endDate.Value.ToString("yyyy-MM-dd") : DateTime.Now.ToString("yyyy-MM-dd"),
            //    url = "/Requests/Request/" + reqId
            //};
            var cal = new CalendarEvent
            {
                title = eventName,
                start = startDate != null ? startDate.Value.ToString("yyyy-MM-dd") : eventDate.ToString("yyyy-MM-dd"),
                end = endDate != null ? endDate.Value.ToString("yyyy-MM-dd") : eventDate.ToString("yyyy-MM-dd"),
                url = "/Requests/Request/" + reqId
            };
            cevents.Add(cal);

            var json = new JavaScriptSerializer().Serialize(cevents);
            calendar.Data = json;
            calendar.Author = author;

            if (insertCalendar)
            {
                context.Calendar.Add(calendar);
            }

            return calendar;
        }

        public List<TitleAuthorAssociation> GetAssociatedTitles(AppUser author)
        {
            return context.TitleAuthorAssociation.Include("Author").Include("Title").Where(a => a.Author.Id == author.Id).ToList();
        }

        public List<Requests> GetRequestsForAuthor(AppUser author)
        {
            return context.Requests
                    .Include("EventOne")
                    .Include("EventOne.TalkType")
                    .Include("EventOne.EventType")
                    .Include("EventOne.TargetSector")
                    .Include("EventTwo")
                    .Include("EventTwo.TalkType")
                    .Include("EventTwo.EventType")
                    .Include("EventTwo.TargetSector")
                    .Include("EventThree")
                    .Include("EventThree.TalkType")
                    .Include("EventThree.EventType")
                    .Include("EventThree.TargetSector")
                    .Include("RequestedAuthor")
                    .Include("PromotedTitle")
                    .Include("TitleForecast")
                    .Include("Region").Where(r => r.RequestedAuthor.Id == author.Id && (r.AdminApproval == true || r.TentativeReason != null)).ToList();
}

        public List<Requests> GetRequestsByYear(string year)
        {
            var yearValue = Int32.Parse(year);

            return context.Requests
                .Include("EventOne")
                .Include("EventTwo")
                .Include("EventThree")
                .Include(e => e.EventOne.TalkType)
                .Include(e => e.EventTwo.TalkType)
                .Include(e => e.EventThree.TalkType)
                .Include("RequestedAuthor")
                .Include(e => e.EventOne.EventType)
                .Include(e => e.EventTwo.EventType)
                .Include(e => e.EventThree.EventType)
                .Include(e => e.EventOne.TargetSector)
                .Include(e => e.EventTwo.TargetSector)
                .Include(e => e.EventThree.TargetSector)
                .Include(e => e.PromotedTitle)
                .Include(e => e.Region)
                .Include(e => e.TitleForecast)
                .Include(e => e.SubmitBy)
                .Where(i => i.SubmitDate.Year == yearValue).ToList();
        }

        public List<Requests> GetRequestsByTitle(string id)
        {
            var titleValue = Int32.Parse(id);

            return context.Requests
                .Include("EventOne")
                .Include("EventTwo")
                .Include("EventThree")
                .Include(e => e.EventOne.TalkType)
                .Include(e => e.EventTwo.TalkType)
                .Include(e => e.EventThree.TalkType)
                .Include("RequestedAuthor")
                .Include(e => e.EventOne.EventType)
                .Include(e => e.EventTwo.EventType)
                .Include(e => e.EventThree.EventType)
                .Include(e => e.EventOne.TargetSector)
                .Include(e => e.EventTwo.TargetSector)
                .Include(e => e.EventThree.TargetSector)
                .Include(e => e.PromotedTitle)
                .Include(e => e.Region)
                .Include(e => e.TitleForecast)
                .Include(e => e.SubmitBy)
                .Where(i => i.PromotedTitle.Id == titleValue).ToList();
        }

        public List<Requests> GetRequestsByRegion(string id)
        {
            var regionId = Int32.Parse(id);

            return context.Requests
                .Include("EventOne")
                .Include("EventTwo")
                .Include("EventThree")
                .Include(e => e.EventOne.TalkType)
                .Include(e => e.EventTwo.TalkType)
                .Include(e => e.EventThree.TalkType)
                .Include("RequestedAuthor")
                .Include(e => e.EventOne.EventType)
                .Include(e => e.EventTwo.EventType)
                .Include(e => e.EventThree.EventType)
                .Include(e => e.EventOne.TargetSector)
                .Include(e => e.EventTwo.TargetSector)
                .Include(e => e.EventThree.TargetSector)
                .Include(e => e.PromotedTitle)
                .Include(e => e.Region)
                .Include(e => e.TitleForecast)
                .Include(e => e.SubmitBy)
                .Where(i => i.Region.Id == regionId).ToList();
        }

        public List<Requests> GetRequestsByEventType(string id)
        {
            var eventId = Int32.Parse(id);

            return context.Requests
                .Include("EventOne")
                .Include("EventTwo")
                .Include("EventThree")
                .Include(e => e.EventOne.TalkType)
                .Include(e => e.EventTwo.TalkType)
                .Include(e => e.EventThree.TalkType)
                .Include("RequestedAuthor")
                .Include(e => e.EventOne.EventType)
                .Include(e => e.EventTwo.EventType)
                .Include(e => e.EventThree.EventType)
                .Include(e => e.EventOne.TargetSector)
                .Include(e => e.EventTwo.TargetSector)
                .Include(e => e.EventThree.TargetSector)
                .Include(e => e.PromotedTitle)
                .Include(e => e.Region)
                .Include(e => e.TitleForecast)
                .Include(e => e.SubmitBy)
                .Where(i => i.EventOne.EventType.Id == eventId || i.EventTwo.EventType.Id == eventId || i.EventThree.EventType.Id == eventId).ToList();
        }

        public List<Requests> GetAllRequests()
        {
            return context.Requests
                .Include("EventOne")
                .Include("EventTwo")
                .Include("EventThree")
                .Include(e => e.EventOne.TalkType)
                .Include(e => e.EventTwo.TalkType)
                .Include(e => e.EventThree.TalkType)
                .Include("RequestedAuthor")
                .Include(e => e.EventOne.EventType)
                .Include(e => e.EventTwo.EventType)
                .Include(e => e.EventThree.EventType)
                .Include(e => e.EventOne.TargetSector)
                .Include(e => e.EventTwo.TargetSector)
                .Include(e => e.EventThree.TargetSector)
                .Include(e => e.PromotedTitle)
                .Include(e => e.Region)
                .Include(e => e.TitleForecast)
                .Include(e => e.SubmitBy)
                .Where(i => i.Id != 0).ToList();
        }

        public Requests CreateWebinar(SubmitWebinarRequestViewModel model, AppUser submitBy)
        {
            Requests req = new Requests
            {
                SubmitBy = GetAppUserFromId(submitBy.Id),
                SubmitDate = DateTime.Now,
                RequestedAuthor = GetAppUserFromId(model.SelectedAuthorId),
                PromotedTitle = GetPromotedTitleById(model.TitlePromotedId),
                TitleForecast = CreateTitleForecast(model.Forecast, model.TitlePromotedId),
                Region = context.Region.FirstOrDefault(a => a.Id == submitBy.Region),
                Declined = false,
                ManagerApproval = false,
                AuthorNotesBySalesRep = model.AuthorNotesBySalesRep,
                NonAuthorNotesBySalesRep = model.NonAuthorNotesBySalesRep,
                SessionDescription=model.SessionDescription,
                LocalAuthorContact=model.LocalAuthorContact

            };

            context.Requests.Add(req);
            context.SaveChanges();
            return req;
        }

        public WebinarCalendar AddRequestToWebinarCalendar(string name, DateTime eventDate, Requests req)
        {
            var calendar = context.WebinarCalendars.FirstOrDefault(x => x.WebinarRoomName == name);
            var insertCalendar = false;

            if (calendar != null)
            {
                JavaScriptSerializer jsSerializer = new JavaScriptSerializer();
                var cevents = jsSerializer.Deserialize<List<CalendarEvent>>(calendar.Data);

                if (cevents == null)
                {
                    cevents = new List<CalendarEvent>();
                }

                var c = new CalendarEvent
                {
                    title = req.EventOne.EventName,
                    start = req.EventOne.EventDate != null ? req.EventOne.EventDate.ToString("yyyy-MM-dd") : DateTime.Now.ToString("yyyy-MM-dd"),
                    end = req.EventOne.EventDate != null ? req.EventOne.EventDate.ToString("yyyy-MM-dd") : DateTime.Now.ToString("yyyy-MM-dd"),
                    url = "/Requests/Request/" + req.Id
                };

                cevents.Add(c);

                var json = new JavaScriptSerializer().Serialize(cevents);
                calendar.Data = json;

                return calendar;
            }
            else
            {
                var c = new CalendarEvent
                {
                    title = req.EventOne.EventName,
                    start = req.EventOne.EventDate != null ? req.EventOne.EventDate.ToString("yyyy-MM-dd") : DateTime.Now.ToString("yyyy-MM-dd"),
                    end = req.EventOne.EventDate != null ? req.EventOne.EventDate.ToString("yyyy-MM-dd") : DateTime.Now.ToString("yyyy-MM-dd"),
                    url = "/Requests/Request/" + req.Id
                };

                var json = new JavaScriptSerializer().Serialize(c);
                calendar.Data = json;

                return calendar;
            }
        }

        public List<AuthorFiles> GetAuthorFiles(AppUser author)
        {
            return context.AuthorFiles.Where(a => a.Author.Id == author.Id).ToList();
        }

        public List<Requests> GetReviewsByDateRange(DateTime fromDate, DateTime toDate, int reg)
        {
            if (reg == 0)
            {
                return context.Requests
                .Include("EventOne")
                .Include("EventTwo")
                .Include("EventThree")
                .Include(e => e.EventOne.TalkType)
                .Include(e => e.EventTwo.TalkType)
                .Include(e => e.EventThree.TalkType)
                .Include("RequestedAuthor")
                .Include(e => e.EventOne.EventType)
                .Include(e => e.EventTwo.EventType)
                .Include(e => e.EventThree.EventType)
                .Include(e => e.EventOne.TargetSector)
                .Include(e => e.EventTwo.TargetSector)
                .Include(e => e.EventThree.TargetSector)
                .Include(e => e.PromotedTitle)
                .Include(e => e.Region)
                .Include(e => e.TitleForecast)
                .Include(e => e.SubmitBy)
                .Where(g => g.EventOne.EventDate >= fromDate && g.EventOne.EventDate <= toDate).ToList();
            }
            else
            {
                return context.Requests
                    .Include("EventOne")
                    .Include("EventTwo")
                    .Include("EventThree")
                    .Include(e => e.EventOne.TalkType)
                    .Include(e => e.EventTwo.TalkType)
                    .Include(e => e.EventThree.TalkType)
                    .Include("RequestedAuthor")
                    .Include(e => e.EventOne.EventType)
                    .Include(e => e.EventTwo.EventType)
                    .Include(e => e.EventThree.EventType)
                    .Include(e => e.EventOne.TargetSector)
                    .Include(e => e.EventTwo.TargetSector)
                    .Include(e => e.EventThree.TargetSector)
                    .Include(e => e.PromotedTitle)
                    .Include(e => e.Region)
                    .Include(e => e.TitleForecast)
                    .Include(e => e.SubmitBy)
                    .Where(g => g.EventOne.EventDate >= fromDate && g.EventOne.EventDate <= toDate && g.Region.Id == reg).ToList();
            }
        }

        public List<Requests> GetReviewsByDateRangeForRep(DateTime fromDate, DateTime toDate, AppUser user)
        {
            return context.Requests
            .Include("EventOne")
            .Include("EventTwo")
            .Include("EventThree")
            .Include(e => e.EventOne.TalkType)
            .Include(e => e.EventTwo.TalkType)
            .Include(e => e.EventThree.TalkType)
            .Include("RequestedAuthor")
            .Include(e => e.EventOne.EventType)
            .Include(e => e.EventTwo.EventType)
            .Include(e => e.EventThree.EventType)
            .Include(e => e.EventOne.TargetSector)
            .Include(e => e.EventTwo.TargetSector)
            .Include(e => e.EventThree.TargetSector)
            .Include(e => e.PromotedTitle)
            .Include(e => e.Region)
            .Include(e => e.TitleForecast)
            .Include(e => e.SubmitBy)
            .Where(g => g.EventOne.EventDate >= fromDate && g.EventOne.EventDate <= toDate && g.SubmitBy.Id == user.Id).ToList();
        }
    }
}
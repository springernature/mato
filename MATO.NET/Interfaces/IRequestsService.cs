using MATO.Classes;
using MATO.NET.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace MATO.NET.Interfaces
{
    public interface IRequestsService
    {
        AppUser GetAppUserFromId(string id);

        Requests CreateRequest(SubmitRequestViewModel model, AppUser submitBy);
        Event CreateEvent(EventViewModel eve, Requests req);
        Requests UpdateRequestWithEvent(Requests request, Event Event, int eventNumber);
        TitleForecast CreateTitleForecast(TitleForecast forecast, int promo);

        EventType GetEventTypeById(int id);
        TargetSector GetTargetSectorById(int id);
        PromotedTitle GetPromotedTitleById(int id);
        TalkType GetTalkTypeById(int id);

        List<EventType> GetEventTypes();
        AppUser GetAuthorDetail(string id);
        List<TitleAuthorAssociation> GetAssociatedTitles(AppUser author);
        List<Country> GetCountryList();
        List<PromotedTitle> GetPromotedTitleList();
        List<PromotedTitle> GetPromotedTitleWithTargetSectors();
        List<TargetSector> GetSectorsTargeted();
        List<TalkType> GetTalkTypes();

        List<Requests> GetRequestsByUser(AppUser user);

        List<Requests> GetFinalisedRequests(string user);
        List<Requests> GetCancelledRequests(string user);
        List<Requests> GetDeclinedRequests(string user);
        List<Requests> GetPendingRequests(string user);
        Requests GetRequestById(int id);

        Region GetRegionById(int id);

        TitleForecast GetTitleForecastById(int id);

        Calendar AddRequestToAuthorCalendar(string id, string eventName, DateTime? startDate, DateTime? endDate,
            int reqId, DateTime eventDate);



        List<Requests> GetRequestsForAuthor(AppUser author);
        List<Requests> GetRequestsByYear(string year);
        List<Requests> GetRequestsByTitle(string id);
        List<Requests> GetRequestsByRegion(string id);
        List<Requests> GetRequestsByEventType(string id);

        List<Requests> GetAllRequests();

        Requests CreateWebinar(SubmitWebinarRequestViewModel model, AppUser submitBy);
        WebinarCalendar AddRequestToWebinarCalendar(string name, DateTime eventDate, Requests req);

        List<AuthorFiles> GetAuthorFiles(AppUser author);

        List<Requests> GetReviewsByDateRange(DateTime fromDate, DateTime toDate, int reg);
        List<Requests> GetReviewsByDateRangeForRep(DateTime fromDate, DateTime toDate, AppUser user);
    }
}
using MATO.Classes;
using MATO.NET.ViewModels.v2Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace MATO.NET.Interfaces
{
    public interface IDecisionsService
    {
        Region IsLoggedInUserRegionManager(string id);
        List<Requests> GetRequestsByRegion(Region region);
        List<Requests> GetRequestsForAdministrator();
        List<Requests> GetRequestsForAuthor(string loggedInId);
        List<Requests> GetReportingRequestsForManager(string loggedIn);
        List<Requests> GetReportingRequestsForAdmins(string loggedIn);


        Requests DeclineRequest(int id);
        Requests ApproveRequest(int id);

        Requests AdminApproveRequest(int id);
        Requests AdminDeclineRequest(int id);

        Requests AuthorApproveRequest(int id);
        Requests AuthorDeclineRequest(int id);

        Requests ResponseWasTentative(AuthorTentativeViewModel model);
    }
}
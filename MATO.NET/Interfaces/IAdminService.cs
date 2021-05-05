using System.Collections.Generic;
using System.Web.Mvc;
using MATO.Classes;
using MATO.NET.ViewModels;
using Microsoft.AspNet.Identity.EntityFramework;

namespace MATO.NET.Interfaces
{
    public interface IAdminService
    {
        List<Region> GetRegions();
        Region GetRegionById(int id);
        string GetRegionalManagerByRegionId(int id);
        List<IdentityRole> GetUserRoles();
        IdentityRole GetRoleByName(string name);
        IdentityRole GetRoleById(string id);
        MultiSelectList GetAuthorsForMultiSelect();
        PromotedTitle CreateTitle(CreateTitleViewModel model);
        Region CreateRegion(CreateRegionViewModel model);
        Calendar GetAuthorCalendar(string id);
        Calendar CreateCalendarForAuthor(string id, string data);
        Calendar UpdateCalendarForAuthor(string id, string data);
        Calendar GetCalendarForAuthor(string id);
        List<EventType> GetEventTypes();
        EventType ToggleEventType(int id);
        EventType AddEventType(string name);
        TargetSector ToggleTargetSector(int id);
        TargetSector AddTargetSector(string name);
        TalkType ToggleTalkType(int id);
        TalkType AddTalkType(string name);
        List<EmailTemplates> GetEmailTemplates();
        EmailTemplates GetEmailTemplateById(int id);
        Calendar DeleteCalendarEvent(string title, AppUser user);
        Region UpdateRegion(ModifyRegionViewModel model);
        PromotedTitle GetTitleById(int id);
        List<AppUser> GetAuthorsByTitleId(int id);
        List<TitleAuthorAssociation> GetAuthorsAndPaymenetType(int id);
        List<PromotedTitle> GetTitles(string id);
         List<TitleAuthorAssociation> GetPaymentType(string id);
        List<PaymentType> GetAllPaymentTypes();
        PaymentType UpdatePaymentType(int RowId, string PaymentId);
        PromotedTitle UpdateTitle(PromotedTitle title, string name, int targetSector, string authors);
        AuditLog CreateAuditLog(int id, string changeItem, string from, string to, string by);
        List<AuditLog> GetAuditLogById(int id);
        List<Region> GetRegionsByManagerId(string id);
        WebinarCalendar GetWebinarCalendar(string name);
        AuthorFiles CreateResourceEntry(string path, AppUser author);
        AuthorFiles GetAuthorFileById(int id);
        AuthorFiles RemoveAuthorFile(AuthorFiles f);
    }
}
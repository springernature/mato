using System;
using MATO.Classes;
using MATO.DataModel;
using MATO.NET.Interfaces;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;
using System.Web.Script.Serialization;
using MATO.NET.ViewModels;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;
using Newtonsoft.Json;

namespace MATO.NET.Services
{
    public class AdminService : IAdminService
    {
        private readonly MATOContext context;
        public IUserService UserService { get; private set; }
        public IRequestsService RequestsService { get; private set; }

        public AdminService(MATOContext _context)
        {
            context = _context;
            UserService = new UserService(context);
            RequestsService = new RequestsService(context);
        }

        public List<Region> GetRegions()
        {
            return context.Region.OrderBy(r => r.Name).ToList();
        }

        public Region GetRegionById(int id)
        {
            return context.Region.FirstOrDefault(r => r.Id == id);
        }

        public string GetRegionalManagerByRegionId(int id)
        {
            var user = context.Region.Include("RegionalManager").FirstOrDefault(a => a.Id == id);
            return user.RegionalManagerId;
        }

        public List<IdentityRole> GetUserRoles()
        {
            var roleStore = new RoleStore<IdentityRole>(context);
            var roleManager = new RoleManager<IdentityRole>(roleStore);
            var roles = roleManager.Roles.ToList();
            return roles;
        }

        public IdentityRole GetRoleById(string id)
        {
            return context.Roles.FirstOrDefault(r => r.Id == id);
        }

        public IdentityRole GetRoleByName(string name)
        {
            return context.Roles.FirstOrDefault(r => r.Name == name);
        }

        public MultiSelectList GetAuthorsForMultiSelect()
        {
            var role = GetRoleById("44f2d63c-a597-46b0-8dc6-cdb4948fae90");
            MultiSelectList authors = new MultiSelectList(context.Users.ToList().OrderBy(i => i.FullName), "Id", "FullName");
            return authors;
        }

        public Region CreateRegion(CreateRegionViewModel model)
        {
            Region r = new Region
            {
                Name = model.Name,
                RegionalManagerId = model.SelectedManagerId,
                Active = true
            };
            context.Region.Add(r);
            return r;
        }

        public Calendar GetAuthorCalendar(string id)
        {
            return context.Calendar.FirstOrDefault(c => c.Author.Id == id);
        }

        public Calendar CreateCalendarForAuthor(string id, string data)
        {
            var author = context.Users.FirstOrDefault(u => u.Id == id);

            Calendar c = new Calendar
            {
                Author = author,
                Data = data
            };

            context.Calendar.Add(c);
            return c;
        }

        public Calendar UpdateCalendarForAuthor(string id, string data)
        {
            var author = context.Users.FirstOrDefault(u => u.Id == id);
            var calendar = context.Calendar.FirstOrDefault(c => c.Author.Id == author.Id);

            if (calendar != null)
            {
                calendar.Data = data;
                return calendar;
            }
            else
            {
                Calendar c = new Calendar { Data = data };
                return c;
            }
        }

        public Calendar GetCalendarForAuthor(string id)
        {
            return context.Calendar.FirstOrDefault(c => c.Author.Id == id);
        }

        public PromotedTitle CreateTitle(CreateTitleViewModel model)
        {
            PromotedTitle title = new PromotedTitle
            {
                Name = model.Name,
                TargetSector = RequestsService.GetTargetSectorById(Int32.Parse(model.SelectedTargetSectorId)),
                Active = true,
            };

            foreach (var a in model.SelectedAuthorIds)
            {
                TitleAuthorAssociation tas = new TitleAuthorAssociation
                {
                    Author = UserService.GetUserById(a),
                    Title = title,
                };

                context.TitleAuthorAssociation.Add(tas);
            }

            context.PromotedTitle.Add(title);

            return title;
        }

        public List<EventType> GetEventTypes()
        {
            return context.EventTypes.OrderBy(e => e.Name).ToList();
        }

        public EventType ToggleEventType(int id)
        {
            var eventType = context.EventTypes.FirstOrDefault(e => e.Id == id);
            eventType.Active = !eventType.Active;

            return eventType;
        }

        public EventType AddEventType(string name)
        {
            EventType e = new EventType
            {
                Name = name,
                Active = true
            };

            context.EventTypes.Add(e);
            return e;
        }

        public TargetSector ToggleTargetSector(int id)
        {
            var targetSector = context.TargetSector.FirstOrDefault(e => e.Id == id);
            targetSector.Active = !targetSector.Active;

            return targetSector;
        }

        public TargetSector AddTargetSector(string name)
        {
            TargetSector e = new TargetSector
            {
                Name = name,
                Active = true
            };

            context.TargetSector.Add(e);
            return e;
        }

        public TalkType AddTalkType(string name)
        {
            TalkType e = new TalkType
            {
                Name = name,
                Active = true
            };

            context.TalkType.Add(e);
            return e;
        }

        public TalkType ToggleTalkType(int id)
        {
            var talkType = context.TalkType.FirstOrDefault(e => e.Id == id);
            talkType.Active = !talkType.Active;

            return talkType;
        }


        public List<EmailTemplates> GetEmailTemplates()
        {
            return context.EmailTemplates.OrderBy(e => e.Name).ToList();
        }

        public EmailTemplates GetEmailTemplateById(int id)
        {
            return context.EmailTemplates.FirstOrDefault(r => r.Id == id);
        }

        public Calendar DeleteCalendarEvent(string title, AppUser user)
        {
            var cal = context.Calendar.FirstOrDefault(c => c.Author.Id == user.Id);
            JavaScriptSerializer jsSerializer = new JavaScriptSerializer();
            var cevents = jsSerializer.Deserialize<List<CalendarEvent>>(cal.Data);

            foreach (var c in cevents.ToList())
            {
                if (c.title == title)
                {
                    cevents.Remove(c);
                }
            }

            var json = new JavaScriptSerializer().Serialize(cevents);
            cal.Data = json;
            cal.Author = user;

            return cal;
        }

        public Region UpdateRegion(ModifyRegionViewModel model)
        {
            var regionId = Int32.Parse(model.SelectedRegionId);
            var region = context.Region.FirstOrDefault(r => r.Id == regionId);

            region.Name = model.Name;
            region.RegionalManagerId = model.SelectedManagerId;
            return region;
        }
        
          public List<PaymentType> GetAllPaymentTypes()
        {
            return context.PaymentTypes.OrderBy(e => e.Name).ToList();

        }
        public PaymentType UpdatePaymentType(int RowId, string PaymentId)
        {
              var paymentTypeId = Int32.Parse(PaymentId);

              var payment = context.PaymentTypes.FirstOrDefault(b => b.Id == paymentTypeId);
              var result = context.TitleAuthorAssociation.FirstOrDefault(b => b.Id == RowId);
                if (result != null)
                {
                    result.PaymentType = payment;
                    context.SaveChanges();
                }
              return payment;
        }
        public PromotedTitle GetTitleById(int id)
        {
            return context.PromotedTitle.Include("TargetSector").FirstOrDefault(t => t.Id == id);
        }
        
        public List<AppUser> GetAuthorsByTitleId(int id)
        {
            var title = context.PromotedTitle.FirstOrDefault(t => t.Id == id);
            var authors = context.TitleAuthorAssociation.Include("Author").Where(t => t.Title.Id == title.Id).ToList();

            var users = new List<AppUser>();
            foreach (var a in authors)
            {
                var user = context.Users.FirstOrDefault(u => u.Id == a.Author.Id);
                users.Add(user);
            }

            return users;
        }
        public List<TitleAuthorAssociation> GetAuthorsAndPaymenetType(int id)
        {
            var title = context.PromotedTitle.FirstOrDefault(t => t.Id == id);
            var authorsWithPymentType = context.TitleAuthorAssociation.Include("Author").Include("PaymentType").Where(t => t.Title.Id == title.Id).ToList();
            return authorsWithPymentType;
        }
        public List<PromotedTitle> GetTitles(string authorid)
        {

            var users = context.TitleAuthorAssociation
               .Where(t => t.Author.Id == authorid)
               .Select(u => u.Title)//not Include
               .ToList();
            return users;

        }
        public List<TitleAuthorAssociation> GetPaymentType(string authorid)
        {
            var payment = context.TitleAuthorAssociation.Include("PaymentType")
              .Where(s => s.Author.Id == authorid).ToList();
           return payment;
        }
        public bool IsIdExist(string author,int titleid)
        {
            if (context.TitleAuthorAssociation.Include("Author").Where(s => s.Title.Id == titleid && s.Author.Id == author).ToList().Count() > 0)
            {
                return true;
            }
            return false;
        }
        public PromotedTitle UpdateTitle(PromotedTitle title, string name, int targetSector, string authors)
        {
            title.Name = name;
            bool flag;
            title.TargetSector = context.TargetSector.FirstOrDefault(x => x.Id == targetSector);
            var list = JsonConvert.DeserializeObject<List<string>>(authors);//selected authors
            var currentAuthors = context.TitleAuthorAssociation.Include("Author").Where(s => s.Title.Id == title.Id).ToList();//all authors
            var deselectedAuthors = currentAuthors.Except(currentAuthors.Where(o => list.Select(s => s).ToList().Contains(o.Author.Id))).ToList();//deselected authors
            foreach (var deselectauthor in deselectedAuthors)
            {
                if (deselectauthor != null) context.TitleAuthorAssociation.Remove(deselectauthor);
            }
            foreach (var a in list)
            {
                var author = context.Users.FirstOrDefault(b => b.Id == a);
                flag = IsIdExist(a, title.Id);

                TitleAuthorAssociation t = new TitleAuthorAssociation();
                t.Author = author;
                t.Title = title;

                if(flag == false)
                {
                    context.TitleAuthorAssociation.Add(t);
                }   
            }
            context.SaveChanges();
            return title;
        }

     
        public AuditLog CreateAuditLog(int id, string changeItem, string from, string to, string by)
        {
            AuditLog aud = new AuditLog
            {
                Request = id,
                ChangeItem = changeItem,
                From = @from,
                To = to,
                UpdateDate = DateTime.Now,
                UpdateBy = @by
            };

            context.AuditLog.Add(aud);
            return aud;
        }

        public List<AuditLog> GetAuditLogById(int id)
        {
            return context.AuditLog.Where(a => a.Request == id).ToList();
        }

        public List<Region> GetRegionsByManagerId(string id)
        {
            return context.Region.Where(x => x.RegionalManagerId == id).ToList();
        }

        public WebinarCalendar GetWebinarCalendar(string name)
        {
            return context.WebinarCalendars.FirstOrDefault(x => x.WebinarRoomName == name);
        }

        public AuthorFiles CreateResourceEntry(string path, AppUser author)
        {
            AuthorFiles f = new AuthorFiles();
            f.Author = author;
            f.FileLocation = path;
            f.Active = true;

            int pos = path.LastIndexOf("/") + 1;

            f.FileName = path.Substring(pos, path.Length - pos);

            context.AuthorFiles.Add(f);
            context.SaveChanges();
            return f;
        }

        public AuthorFiles GetAuthorFileById(int id)
        {
            return context.AuthorFiles.FirstOrDefault(x => x.Id == id);
        }

        public AuthorFiles RemoveAuthorFile(AuthorFiles f)
        {
            context.AuthorFiles.Remove(f);
            context.SaveChanges();
            return f;
        }
    }
}
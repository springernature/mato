using MATO.Classes;
using MATO.DataModel;
using MATO.NET.Interfaces;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using Microsoft.Ajax.Utilities;

namespace MATO.NET.Services
{
    public class UserService : IUserService
    {
        private readonly MATOContext context;

        public UserService(MATOContext _context)
        {
            context = _context;
        }

        public AppUser GetUserById(string id)
        {
            return context.Users.FirstOrDefault(a => a.Id == id);
        }

        public AppUser GetUserByEmailAddress(string email)
        {
            return context.Users.FirstOrDefault(u => u.Email == email);
        }

        public List<AppUser> GetApplicationUsersInRole(string roleName)
        {
            var role = context.Roles.FirstOrDefault(x => x.Name == roleName);
            List<AppUser> users = context.Users.Where(x => x.Roles.Select(y => y.RoleId).Contains(role.Id)).OrderBy(x => x.FullName).ToList();
            return users;
        }

        public string GetUserRoleByUserId(string id)
        {
            var roleId = "";
            var role = context.Users.FirstOrDefault(x => x.Roles.Select(y => y.UserId).Contains(id));
            foreach (var r in role.Roles)
            {
                roleId = r.RoleId;
            } 

            var roleName = context.Roles.FirstOrDefault(x => x.Id == roleId);
            return roleName.Name;
        }

        public AppUser DeleteUser(string id)
        {
            var authorsList = context.TitleAuthorAssociation.Include("Author").Where(s => s.Author.Id == id).ToList();
            foreach (var a in authorsList)
            {
                context.TitleAuthorAssociation.Remove(a);
            }
            var calendarsRows = context.Calendar.Where(s => s.Author.Id == id).ToList();
            foreach (var c in calendarsRows)
            {
                context.Calendar.Remove(c);
            }
            var requestRows = context.Requests.Include("RequestedAuthor").Where(a => a.RequestedAuthor.Id == id).ToList();
            foreach (var r in requestRows)
            {
                context.Requests.Remove(r);
            }
            var AuthorFilesRows = context.AuthorFiles.Where(s => s.Author.Id == id).ToList();
            foreach (var af in AuthorFilesRows)
            {
                context.AuthorFiles.Remove(af);
            }
            var user = context.Users.FirstOrDefault(o => o.Id == id);
            context.Users.Remove(user);
            return user;
        }
       
        public List<AppUser> GetAllUsers()
        {
            return context.Users.Where(o => o.Id != null).OrderBy(o => o.FirstName).ToList();
        }

        public AppUser UpdateUser(AppUser user, string FirstName, string LastName, string Email, int Region,
            string Country,
            string Role)
        {
            user.Country = Country;
            user.FirstName = FirstName;
            user.LastName = LastName;
            user.Email = Email;
            user.Region = Region;
            user.UserName = Email;
            user.FullName = FirstName + " " + LastName;

            context.SaveChanges();

            return user;
        }

        public Calendar GetUserCalendarByUser(AppUser user)
        {
            return context.Calendar.FirstOrDefault(u => u.Author.Id == user.Id);
        }

        public AuthorDetails CreateAuthorDetailsEntry(AuthorDetails aud)
        {
            context.AuthorDetails.Add(aud);
            return aud;
        }

        public AuthorDetails GetAuthorDetails(string id)
        {
            return context.AuthorDetails.FirstOrDefault(a => a.AuthorId == id);
        }

        public AppUser GetSuperAdmin(string roleName)
        {
            var role = context.Roles.FirstOrDefault(x => x.Name == roleName);
            var user = context.Users.FirstOrDefault(x => x.Roles.Select(y => y.RoleId).Contains(role.Id));
            return user;
        }

        public WebinarCalendar GetWebinarCalendar()
        {
            return context.WebinarCalendars.FirstOrDefault(u => u.Id == 1);
        }
    }
}
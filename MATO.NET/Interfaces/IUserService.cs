using MATO.Classes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace MATO.NET.Interfaces
{
    public interface IUserService
    {
        AppUser GetUserById(string id);
        AppUser GetUserByEmailAddress(string email);
        List<AppUser> GetApplicationUsersInRole(string roleName);
        string GetUserRoleByUserId(string id);
        List<AppUser> GetAllUsers();
        AppUser UpdateUser(AppUser user, string FirstName, string LastName, string Email, int Region, string Country, string Role);
        AppUser DeleteUser(string id);
        Calendar GetUserCalendarByUser(AppUser user);
        AuthorDetails CreateAuthorDetailsEntry(AuthorDetails aud);
        AuthorDetails GetAuthorDetails(string id);
        AppUser GetSuperAdmin(string roleName);
        WebinarCalendar GetWebinarCalendar();
    }
}
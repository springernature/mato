using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using MATO.Classes;
using MATO.DataModel;
using MATO.NET.Models;
using MATO.NET.Persistence;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;
using Microsoft.AspNet.Identity.Owin;

namespace MATO.NET.Controllers
{
    public class BulkImportController : Controller
    {
        private readonly IUnitOfWork _unitOfWork;
        private ApplicationUserManager _userManager;
        MATOContext context = new MATOContext();

        public BulkImportController()
        {
            _unitOfWork = new UnitOfWork(new MATOContext());
        }

        public BulkImportController(ApplicationUserManager userManager)
        {
            UserManager = userManager;
        }

        public ApplicationUserManager UserManager
        {
            get { return _userManager ?? HttpContext.GetOwinContext().GetUserManager<ApplicationUserManager>(); }
            private set { _userManager = value; }
        }

        public ActionResult Users()
        {
            var newUsers = from row in System.IO.File.ReadLines(@"C:\mato.csv")
                           let column = row.Split(',')
                           select new UserData
                           {
                               FirstName = column[0],
                               LastName = column[1],
                               EmailAddress = column[2],
                               Role = column[3],
                               Region = Int32.Parse(column[4])
                           };

            foreach (var p in newUsers)
            {
                if(!string.IsNullOrEmpty(p.FirstName))
                CreateBulkIdentityUser(p.FirstName, p.LastName, p.EmailAddress, p.Role, p.Region);
            }

            return null;
        }

        public class UserData
        {
            public string FirstName { get; set; }
            public string LastName { get; set; }
            public string EmailAddress { get; set; }
            public int Region { get; set; }
            public string Role { get; set; }
        }


        public void CreateBulkIdentityUser(string firstName, string lastName, string emailAddress, string role, int region)
        {
            PasswordHasher passwordHasher = new PasswordHasher();
            var user = _unitOfWork.UserService.GetUserByEmailAddress(emailAddress + "fake");

            if (user == null)
            {
                user = new AppUser()
                {
                    UserName = emailAddress,
                    FirstName = firstName,
                    LastName = lastName,
                    FullName = firstName + " " + lastName,
                    Email = emailAddress + "fake",
                    EmailConfirmed = false,
                    PhoneNumberConfirmed = false,
                    TwoFactorEnabled = false,
                    LockoutEnabled = false,
                    PasswordHash = passwordHasher.HashPassword("Welcome.1"),
                    SecurityStamp = Guid.NewGuid().ToString().ToLower(),
                    Region = region
                };

                context.Users.Add(user);
                context.SaveChanges();

                var roleId = context.Roles.FirstOrDefault(r => r.Name == role);
                if (roleId != null)
                {
                    UserManager.AddToRole(user.Id, roleId.Name);
                    context.SaveChanges();
                }
                else
                {
                    context.Users.Remove(user);
                    context.SaveChanges();
                }

            }

            
        }
    }
}

using System;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.Owin;
using Microsoft.Owin;
using Microsoft.Owin.Security.Cookies;
using Microsoft.Owin.Security.Google;
using Owin;
using MATO.NET.Models;
using MATO.DataModel;
using MATO.Classes;
using System.Web.Configuration;
using System.Configuration;
using Microsoft.AspNet.Identity.EntityFramework;
using System.Collections.Generic;
using System.Linq;

namespace MATO.NET
{
    public partial class Startup
    {
        // For more information on configuring authentication, please visit http://go.microsoft.com/fwlink/?LinkId=301864
        public void ConfigureAuth(IAppBuilder app)
        {
            // Configure the db context, user manager and signin manager to use a single instance per request
            app.CreatePerOwinContext(() => new MATOContext());
            app.CreatePerOwinContext<ApplicationUserManager>(ApplicationUserManager.Create);
            app.CreatePerOwinContext<ApplicationSignInManager>(ApplicationSignInManager.Create);

            // Enable the application to use a cookie to store information for the signed in user
            // and to use a cookie to temporarily store information about a user logging in with a third party login provider
            // Configure the sign in cookie
            SessionStateSection sessionStateSection = ConfigurationManager.GetSection("system.web/sessionState") as SessionStateSection;
            app.UseCookieAuthentication(new CookieAuthenticationOptions
            {
                AuthenticationType = DefaultAuthenticationTypes.ApplicationCookie,
                LoginPath = new PathString("/Account/Login"),
                CookieName = sessionStateSection.CookieName + "_Application",
                Provider = new CookieAuthenticationProvider
                {
                    // Enables the application to validate the security stamp when the user logs in.
                    // This is a security feature which is used when you change a password or add an external login to your account.  
                    OnValidateIdentity = SecurityStampValidator.OnValidateIdentity<ApplicationUserManager, AppUser>(
                        validateInterval: TimeSpan.FromMinutes(30),
                        regenerateIdentity: (manager, user) => user.GenerateUserIdentityAsync(manager))
                }
            });
            app.UseExternalSignInCookie(DefaultAuthenticationTypes.ExternalCookie);

            // Enables the application to temporarily store user information when they are verifying the second factor in the two-factor authentication process.
            app.UseTwoFactorSignInCookie(DefaultAuthenticationTypes.TwoFactorCookie, TimeSpan.FromMinutes(5));

            // Enables the application to remember the second login verification factor such as phone or email.
            // Once you check this option, your second step of verification during the login process will be remembered on the device where you logged in from.
            // This is similar to the RememberMe option when you log in.
            app.UseTwoFactorRememberBrowserCookie(DefaultAuthenticationTypes.TwoFactorRememberBrowserCookie);

            // Uncomment the following lines to enable logging in with third party login providers
            //app.UseMicrosoftAccountAuthentication(
            //    clientId: "",
            //    clientSecret: "");

            //app.UseTwitterAuthentication(
            //   consumerKey: "",
            //   consumerSecret: "");

            //app.UseFacebookAuthentication(
            //   appId: "",
            //   appSecret: "");

            //app.UseGoogleAuthentication(new GoogleOAuth2AuthenticationOptions()
            //{
            //    ClientId = "",
            //    ClientSecret = ""
            //});

            CreateDefaultData();
            CreateRolesandUsers();
        }

        private void CreateRolesandUsers()
        {
            MATOContext context = new MATOContext();

            var roleManager = new RoleManager<IdentityRole>(new RoleStore<IdentityRole>(context));
            var UserManager = new UserManager<AppUser>(new UserStore<AppUser>(context));


            // In Startup iam creating first Admin Role and creating a default Admin User    
            if (!roleManager.RoleExists("Admin"))
            {

                // first we create Admin role
                var role = new Microsoft.AspNet.Identity.EntityFramework.IdentityRole();
                role.Name = "Admin";
                roleManager.Create(role);

                //Here we create a Admin super user who will maintain the website                  

                var user = new AppUser();
                user.UserName = "tristanh@ribbonfish.co.uk";
                user.FirstName = "Tristan";
                user.LastName = "Hudson";
                user.FullName = user.FirstName + " " + user.LastName;
                user.Email = "tristanh@ribbonfish.co.uk";
                user.Region = context.Region.FirstOrDefault().Id;
                string userPWD = "Oxford28!";

                var chkUser = UserManager.Create(user, userPWD);

                //Add default User to Role Admin   
                if (chkUser.Succeeded)
                {
                    var result1 = UserManager.AddToRole(user.Id, "Admin");
                }
            }

            // creating Creating Manager role    
            if (!roleManager.RoleExists("Manager"))
            {
                var role = new Microsoft.AspNet.Identity.EntityFramework.IdentityRole();
                role.Name = "Manager";
                roleManager.Create(role);
            }

            // creating Creating Manager role    
            if (!roleManager.RoleExists("SuperAdmin"))
            {
                var role = new Microsoft.AspNet.Identity.EntityFramework.IdentityRole {Name = "SuperAdmin"};
                roleManager.Create(role);
            }


            if (!roleManager.RoleExists("Author"))
            {
                var role = new Microsoft.AspNet.Identity.EntityFramework.IdentityRole {Name = "Author"};
                roleManager.Create(role);

                var user = new AppUser
                {
                    FirstName = "Tommy",
                    LastName = "Smith"
                };

                user.FullName = user.FirstName + " " + user.LastName;
                user.Email = "tommys@ribbonfish.co.uk";
                user.UserName = "tommys@ribbonfish.co.uk";
                user.Region = context.Region.FirstOrDefault().Id;
                string userPWD = "Oxford28!";

                var chkUser = UserManager.Create(user, userPWD);

                //Add default User to Role Admin   
                if (chkUser.Succeeded)
                {
                    var result1 = UserManager.AddToRole(user.Id, "Author");
                }

                var user2 = new AppUser();
                user.FirstName = "David";
                user.LastName = "Smith";
                user.FullName = user.FirstName + " " + user.LastName;
                user.Email = "davids@ribbonfish.co.uk";
                user.UserName = "davids@ribbonfish.co.uk";
                user.Region = context.Region.FirstOrDefault().Id;
                string userPWD2 = "Oxford28!";

                var chkUser2 = UserManager.Create(user2, userPWD2);

                //Add default User to Role Admin   
                if (chkUser2.Succeeded)
                {
                    var result1 = UserManager.AddToRole(user2.Id, "Author");
                }
            }

            if (!roleManager.RoleExists("SalesRep"))
            {
                var role = new Microsoft.AspNet.Identity.EntityFramework.IdentityRole();
                role.Name = "SalesRep";
                roleManager.Create(role);
            }
        }

        private void CreateDefaultData()
        {
            MATOContext context = new MATOContext();

            var eventTypes = context.EventTypes.Any();
            var titlePromoted = context.PromotedTitle.Any();
            var sectorTargeted = context.TargetSector.Any();
            var talkType = context.TalkType.Any();
            var regions = context.Region.Any();

            if(!regions)
            {
                List<Region> r = new List<Region>();

                Region reg1 = new Region { 
                Name = "Europe",
                Active = true,
                RegionalManagerId = null
                };
                r.Add(reg1);

                Region reg2 = new Region
                {
                    Name = "North America",
                    Active = true,
                    RegionalManagerId = null,
                };
                r.Add(reg2);

                foreach (var regi in r)
                {
                    context.Region.Add(regi);
                }

                context.SaveChanges();

            }

            if (!eventTypes)
            {
                List<EventType> et = new List<EventType>();

                EventType et1 = new EventType
                {
                    Name = "Macmillan Day",
                    Active = true
                };
                et.Add(et1);

                EventType et2 = new EventType
                {
                    Name = "External: Webinar",
                    Active = true
                };
                et.Add(et2);

                EventType et3 = new EventType
                {
                    Name = "Webinar requiring travel",
                    Active = true
                };
                et.Add(et3);

                EventType et4 = new EventType
                {
                    Name = "Other",
                    Active = true
                };
                et.Add(et4);

                EventType et5 = new EventType
                {
                    Name = "Internal Sales Conference",
                    Active = true
                };
                et.Add(et5);

                foreach (var e in et)
                {
                    context.EventTypes.Add(e);
                }

                context.SaveChanges();
            }

            if (!titlePromoted)
            {
                List<PromotedTitle> pt = new List<PromotedTitle>();

                PromotedTitle pt1 = new PromotedTitle
                {
                    Name = "The Angry Tiger 1 SB",
                    Active = true
                };
                pt.Add(pt1);

                PromotedTitle pt2 = new PromotedTitle
                {
                    Name = "The Angry Tiger 2 SB",
                    Active = true
                };
                pt.Add(pt2);

                PromotedTitle pt3 = new PromotedTitle
                {
                    Name = "The Angry Tiger 3 SB",
                    Active = true
                };
                pt.Add(pt3);

                foreach (var e in pt)
                {
                    context.PromotedTitle.Add(e);
                }

                context.SaveChanges();
            }

            if (!sectorTargeted)
            {
                List<TargetSector> ts = new List<TargetSector>();

                TargetSector ts1 = new TargetSector
                {
                    Name = "Primary",
                    Active = true
                };
                ts.Add(ts1);

                TargetSector ts2 = new TargetSector
                {
                    Name = "Secondary",
                    Active = true
                };
                ts.Add(ts2);

                TargetSector ts3 = new TargetSector
                {
                    Name = "Pre-Primary",
                    Active = true
                };
                ts.Add(ts3);

                foreach (var e in ts)
                {
                    context.TargetSector.Add(e);
                }

                context.SaveChanges();
            }

            if (!talkType)
            {
                List<TalkType> tt = new List<TalkType>();

                TalkType tt1 = new TalkType
                {
                    Name = "Whisper",
                    Active = true
                };
                tt.Add(tt1);

                TalkType tt2 = new TalkType
                {
                    Name = "Shout",
                    Active = true
                };
                tt.Add(tt2);

                TalkType tt3 = new TalkType
                {
                    Name = "Commanding Roar",
                    Active = true
                };
                tt.Add(tt3);

                foreach (var e in tt)
                {
                    context.TalkType.Add(e);
                }

                context.SaveChanges();
            }


        }
    }
}
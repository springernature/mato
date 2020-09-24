using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using MATO.Classes;
using Microsoft.AspNet.Identity.EntityFramework;

namespace MATO.NET.ViewModels
{
    public class ModifyUserViewModel
    {
        [Display(Name = "Users")]
        public string SelectedUserId { get; set; }
        public IEnumerable<AppUser> Users { get; set; }

        [Display(Name = "First Name")]
        public string FirstName { get; set; }

        [Display(Name = "Last Name")]
        public string LastName { get; set; }

        [Display(Name = "Email Address")]
        public string EmailAddress { get; set; }

        [Display(Name = "Region")]
        public int RegionId { get; set; }
        public IEnumerable<Region> Region { get; set; }

        [Display(Name = "Country")]
        public string Country { get; set; }

        [Display(Name = "User Group")]
        public string UserGroupId { get; set; }
        public IEnumerable<IdentityRole> Roles { get; set; }

        public AuthorDetails AuthorDetails { get; set; }
    }
}
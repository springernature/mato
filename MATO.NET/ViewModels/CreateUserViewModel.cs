using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using MATO.Classes;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;

namespace MATO.NET.ViewModels
{
    public class CreateUserViewModel
    {
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

        [Display(Name = "Author Type")]
        public string AuthorType { get; set; }

        [Display(Name = "Address Line 1")]
        public string AuthorAddressLine1 { get; set; }
        [Display(Name = "Address Line 2")]
        public string AuthorAddressLine2 { get; set; }
        [Display(Name = "Address Line 3")]
        public string AuthorAddressLine3 { get; set; }

        [Display(Name = "Author Notes")]
        public string AuthorNotes { get; set; }
    }
}
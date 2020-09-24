using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using MATO.Classes;
using Microsoft.AspNet.Identity.EntityFramework;

namespace MATO.NET.ViewModels
{
    public class ModifyCalendarEventsViewModel
    {
        [Display(Name = "Users")]
        public string SelectedUserId { get; set; }
        public IEnumerable<AppUser> Users { get; set; }
    }
}
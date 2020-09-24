using MATO.Classes;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace MATO.NET.ViewModels
{
    public class CreateCalendarEventViewModel
    {
        [Display(Name = "Author")]
        public string SelectedAuthorId { get; set; }
        public IEnumerable<AppUser> Authors { get; set; }

        [Display(Name = "Event Name")]
        public string EventName { get; set; }

        [Display(Name = "Start Date")]
        public DateTime? StartDate { get; set; }

        [Display(Name = "End Date")]
        public DateTime? EndDate { get; set; }
    }
}
using MATO.Classes;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace MATO.NET.ViewModels
{
    public class SubmitWebinarRequestViewModel
    {
        public int Id { get; set; }
        public AppUser CurrentUser { get; set; }
        public Region UserRegion { get; set; }

        [Display(Name = "Author")]
        public string SelectedAuthorId { get; set; }
        public IEnumerable<AppUser> Authors { get; set; }

        public string PersonDetails { get; set; }

        [Display(Name = "Title Promoted")]
        public int TitlePromotedId { get; set; }
        public IEnumerable<PromotedTitle> PromotedTitle { get; set; }

        public TitleForecast Forecast { get; set; }

        public int Region { get; set; }

        public EventViewModel EventOne { get; set; }

        [Display(Name = "Author Visible Notes")]
        public string AuthorNotesBySalesRep { get; set; }

        [Display(Name = "Author Not-visible Notes")]
        public string NonAuthorNotesBySalesRep { get; set; }

        [Display(Name = "Session Description")]
        public string SessionDescription { get; set; }
        [Display(Name = "Local Author Contact")]
        public string LocalAuthorContact { get; set; }
    }
}
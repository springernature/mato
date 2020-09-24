using MATO.Classes;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace MATO.NET.ViewModels
{
    public class SubmitRequestViewModel
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

        [Display(Name = "Inbound Date")]
        public DateTime? InboundDate { get; set; }

        [Display(Name = "Outbound Date")]
        public DateTime? OutboundDate { get; set; }

        [Display(Name = "Inbound From")]
        public string Inbound { get; set; }

        [Display(Name = "Outbound From")]
        public string Outbound { get; set; }

        public int Region { get; set; }

        public EventViewModel EventOne { get; set; }
        public EventViewModel EventTwo { get; set; }
        public EventViewModel EventThree { get; set; }

        [Display(Name = "Author Visible Notes")]
        public string AuthorNotesBySalesRep { get; set; }

        [Display(Name = "Author Not-visible Notes")]
        public string NonAuthorNotesBySalesRep { get; set; }
    }
}
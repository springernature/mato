using MATO.Classes;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace MATO.NET.ViewModels
{
    public class ViewRequestViewModel
    {
        public int Id { get; set; }

        [Display(Name = "Who Submit")]
        public AppUser WhoSubmit { get; set; }

        [Display(Name = "Author")]
        public AppUser Author { get; set; }

        public string PersonDetails { get; set; }

        [Display(Name = "Title Promoted")]
        public PromotedTitle PromotedTitle { get; set; }

        public TitleForecast Forecast { get; set; }

        [Display(Name = "Inbound Date")]
        public DateTime? InboundDate { get; set; }

        [Display(Name = "Outbound Date")]
        public DateTime? OutboundDate { get; set; }

        [Display(Name = "Inbound From")]
        public string Inbound { get; set; }

        [Display(Name = "Outbound From")]
        public string Outbound { get; set; }

        public Event EventOne { get; set; }
        public Event EventTwo { get; set; }
        public Event EventThree { get; set; }

        [Display(Name = "Author Visible Notes")]
        public string AuthorNotesBySalesRep { get; set; }

        [Display(Name = "Author Not-visible Notes")]
        public string NonAuthorNotesBySalesRep { get; set; }

        public Region Region { get; set; }

        public bool Finalised { get; set; }
        public DateTime? FinalisedDate { get; set; }

        public bool ManagerApproved { get; set; }
        public DateTime? ManagerApprovalDate { get; set; }

        public bool AdminApproved { get; set; }
        public DateTime? AdminApprovalDate { get; set; }

        public bool Declined { get; set; }
        public DateTime? DeclinedDate { get; set; }

        public DateTime SubmitDate { get; set; }
    }
}
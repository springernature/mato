using System;

namespace MATO.Classes
{
    public class Requests
    {
        public int Id { get; set; }
        public AppUser SubmitBy { get; set; }
        public DateTime SubmitDate { get; set; }

        public AppUser RequestedAuthor { get; set; }
        public PromotedTitle PromotedTitle { get; set; }
        public TitleForecast TitleForecast { get; set; }

        public DateTime? InboundDate { get; set; }
        public DateTime? OutboundDate { get; set; }
        public string Inbound { get; set; }
        public string Outbound { get; set; }

        public Event EventOne { get; set; }
        public Event EventTwo { get; set; }
        public Event EventThree { get; set; }

        public bool ManagerApproval { get; set; }
        public DateTime? ManagerApprovalDate { get; set; }

        public bool AdminApproval { get; set; }
        public DateTime? AdminApprovalDate { get; set; }

        public bool Finalised { get; set; }
        public DateTime? FinalisedDate { get; set; }

        public bool Cancelled { get; set; }
        public DateTime? CancelledDate { get; set; }

        public bool Declined { get; set; }
        public DateTime? DeclinedDate { get; set; }

        public Region Region { get; set; }

        public string AuthorNotesBySalesRep { get; set; }
        public string NonAuthorNotesBySalesRep { get; set; }
        public string AuthorNotesByAdmin { get; set; }
        public string NonAuthorNotesByAdmin { get; set; }

        public bool Complete { get; set; }

        public string TentativeReason { get; set; }
    }
}

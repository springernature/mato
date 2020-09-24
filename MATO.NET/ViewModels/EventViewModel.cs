using MATO.Classes;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace MATO.NET.ViewModels
{
    public class EventViewModel
    {
        [Display(Name = "Event Name")]
        public string EventName { get; set; }

        [Display(Name = "Event City")]
        public string EventCity { get; set; }

        [Display(Name = "Event Type")]
        public int SelectedEventId { get; set; }
        public IEnumerable<EventType> EventType { get; set; }

        [Display(Name = "Event Date")]
        public DateTime? EventDate { get; set; }

        [Display(Name = "Sector Targeted")]
        public int SectorTargetedId { get; set; }
        public IEnumerable<TargetSector> TargetSector { get; set; }

        [Display(Name = "Expected Turnout")]
        public int? ExpectedTurnout { get; set; }

        [Display(Name = "Talk Type")]
        public int TalkTypeId { get; set; }
        public IEnumerable<TalkType> TalkType { get; set; }

    }
}
using MATO.Classes;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace MATO.NET.ViewModels
{
    public class EventTypeViewModel
    {
        [Display(Name = "Event Types")]
        public string SelectedEventTypeId { get; set; }
        public IEnumerable<EventType> EventTypes { get; set; }
    }
}
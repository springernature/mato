using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using MATO.Classes;

namespace MATO.NET.ViewModels
{
    public class PendingDecisionsViewModel
    {
        public List<Requests> Decisions { get; set; }
        public List<Requests> AdminDecisions { get; set; }
        public List<Requests> AuthorDecisions { get; set; }
        public List<Requests> ClosedDecisions { get; set; }
    }
}
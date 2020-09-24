using MATO.Classes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace MATO.NET.ViewModels
{
    public class ManageRequestViewModel
    {
        public List<Requests> FinalisedRequests { get; set; }
        public List<Requests> PendingRequests { get; set; }
        public List<Requests> CancelledRequests { get; set; }
        public List<Requests> GetDeclinedRequests { get; set; }
    }
}
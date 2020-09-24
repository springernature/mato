using MATO.Classes;
using System.Collections.Generic;

namespace MATO.NET.ViewModels
{
    public class UserHomeScreenViewModel
    {
        public List<Requests> MyRequests { get; set; }
        public AppUser User { get; set; }

        public List<Requests> UserRequests { get; set; }
    }
}
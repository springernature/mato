using MATO.Classes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace MATO.NET.ViewModels.v2Models
{
    public class IndividualAuthorViewModel
    {
        public AppUser Author { get; set; }
        public Calendar Calendar { get; set; }
        public List<AuthorFiles> Files { get; set; }
        public AuthorDetails AuthorDetail { get; set; }
    }
}
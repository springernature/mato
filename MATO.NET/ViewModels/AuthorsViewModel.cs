using MATO.Classes;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace MATO.NET.ViewModels
{
    public class AuthorsViewModel
    {
        [Display(Name = "Author")]
        public string SelectedAuthorId { get; set; }
        public IEnumerable<AppUser> Authors { get; set; }
    }
}
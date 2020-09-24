using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MATO.Classes
{
    public class AuthorDetails
    {
        public int Id { get; set; }
        public string AuthorId { get; set; }

        [Display(Name = "Address Line 1")]
        public string AddressLine1 { get; set; }
        [Display(Name = "Address Line 2")]
        public string AddressLine2 { get; set; }
        [Display(Name = "Address Line 3")]
        public string AddressLine3 { get; set; }

        public string AuthorType { get; set; }
        public string AuthorNotes { get;set; }

        public string AuthorPicture { get; set; }
    }
}

using MATO.Classes;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace MATO.NET.ViewModels
{
    public class EmailListViewModel
    {
        [Display(Name = "Email")]
        public string SelectedEmailId { get; set; }
        public IEnumerable<EmailTemplates> EmailTemplates { get; set; }

        [Display(Name = "Name")]
        public string Name { get; set; }

        [Display(Name = "Body")]
        public string Body { get; set; }

        public bool Active { get; set; }
    }
}
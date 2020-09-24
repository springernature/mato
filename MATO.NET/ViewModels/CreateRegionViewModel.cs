using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using MATO.Classes;

namespace MATO.NET.ViewModels
{
    public class CreateRegionViewModel
    {
        [Display(Name = "Name")]
        public string Name { get; set; }

        [Display(Name = "Regional Manager")]
        public string SelectedManagerId { get; set; }
        public IEnumerable<AppUser> RegionalManager { get; set; }

        public bool Active { get; set; }
    }
}
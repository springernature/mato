using MATO.Classes;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace MATO.NET.ViewModels
{
    public class RegionsViewModel
    {
        [Display(Name = "Regions")]
        public string SelectedRegionId { get; set; }
        public IEnumerable<Region> Regions { get; set; }
    }
}
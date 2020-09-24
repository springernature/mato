using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using MATO.Classes;

namespace MATO.NET.ViewModels
{
    public class ModifyTitleViewModel
    {
        [Display(Name = "Titles")]
        public string SelectedTitleId { get; set; }
        public IEnumerable<PromotedTitle> Titles { get; set; }

        public string Name { get; set; }

        [Display(Name = "Target Sector")]
        public string SelectedTargetSectorId { get; set; }
        public IEnumerable<TargetSector> TargetSector { get; set; }

        [Display(Name = "Author(s)")]
        public List<string> SelectedAuthorIds { get; set; }
        public MultiSelectList Authors { get; set; }
     
    }
}
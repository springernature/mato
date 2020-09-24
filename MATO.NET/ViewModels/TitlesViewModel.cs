using MATO.Classes;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace MATO.NET.ViewModels
{
    public class TitlesViewModel
    {
        [Display(Name = "Titles")]
        public string SelectedTitleId { get; set; }
        public IEnumerable<PromotedTitle> Titles { get; set; }
    }

}
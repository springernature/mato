using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MATO.Classes
{
    public class TitleForecast
    {
        public int Id { get; set; }

        [Display(Name = "Year 1 Forecast")]
        public int Year1 { get; set; }

        [Display(Name = "Year 2 Forecast")]
        public int Year2 { get; set; }

        [Display(Name = "Year 3 Forecast")]
        public int Year3 { get; set; }

        public PromotedTitle PromotedTitle { get; set; }

    }
}

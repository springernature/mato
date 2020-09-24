using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MATO.Classes
{
    public class TitleAuthorAssociation
    {
        public int Id { get; set; }
        public AppUser Author { get; set; }
        public PromotedTitle Title { get; set; }
        public PaymentType PaymentType { get; set; }
    }
}

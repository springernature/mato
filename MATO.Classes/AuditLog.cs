using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MATO.Classes
{
    public class AuditLog
    {
        public int Id { get; set; }
        public int Request { get; set; }
        public string ChangeItem { get; set; }
        public string From { get; set; }
        public string To { get; set; }
        public DateTime UpdateDate { get; set; }
        public string UpdateBy { get; set; }
    }
}

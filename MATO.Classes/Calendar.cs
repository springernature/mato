using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MATO.Classes
{
    public class Calendar
    {
        public int Id { get; set; }
        public AppUser Author { get; set; }
        public string Data { get; set; }
    }
}

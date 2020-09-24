using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MATO.Classes
{
    public class Region
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string RegionalManagerId { get; set; }
        public bool Active { get; set; }
    }
}

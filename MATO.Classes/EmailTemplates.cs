﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MATO.Classes
{
    public class EmailTemplates
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Html { get; set; }
        public bool Active { get; set; }
    }
}

﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MATO.Classes
{
    public class AuthorFiles
    {
        public int Id { get; set; }
        public AppUser Author { get; set; }
        public string FileLocation { get; set; }
        public string FileName { get; set; }
        public bool Active { get; set; }
    }
}

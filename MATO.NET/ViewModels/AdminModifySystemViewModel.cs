using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using MATO.Classes;

namespace MATO.NET.ViewModels
{
    public class AdminModifySystemViewModel
    {
        public List<EventType> EventTypes { get; set; }
        public List<TargetSector> TargetSectors { get; set; }
        public List<TalkType> TalkTypes { get; set; }
    }
}
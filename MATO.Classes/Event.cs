using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MATO.Classes
{
    public class Event
    {
        public int Id { get; set; }
        public string EventName { get; set; }
        public string EventCity { get; set; }
        public DateTime EventDate { get; set; }
        public EventType EventType { get; set; }
        public TargetSector TargetSector { get; set; }
        public TalkType TalkType { get; set; }
        public int? ExpectedTurnout { get; set; }
    }
}

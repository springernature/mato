using System.Collections.Generic;

namespace MATO.Classes
{
    public class PromotedTitle
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public TargetSector TargetSector { get; set; }
        public bool Active { get; set; }
    }
}

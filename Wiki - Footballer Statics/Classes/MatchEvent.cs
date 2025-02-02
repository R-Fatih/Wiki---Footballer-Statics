using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Wiki___Footballer_Statics.Enums;

namespace Wiki___Footballer_Statics.Classes
{
    public class MatchEvent
    {
        public int Id { get; set; }
        public int MatchId { get; set; }
        public int FirstActorPlayerId { get; set; }
        public int SecondActorPlayerId { get; set; }
        public EventDetail EventDetail { get; set; }
        public int Minute { get; set; }

    }
}

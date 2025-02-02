using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Wiki___Footballer_Statics.Classes
{
    public class Match
    {
        public int Id { get; set; }
        public int MId { get; set; }
        public string HomeTeam { get; set; }
        public string AwayTeam { get; set; }
        public string HalfTime { get; set; }
        public string MatchResult { get; set; }
        public IList<MatchEvent> MatchEvents { get; set; }
        public IList<MatchLineUp> HomeLineUp { get; set; }
    }
}

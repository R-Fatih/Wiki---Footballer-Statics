using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Wiki___Footballer_Statics.Responses
{
    public class PlayerStatisticsView
    {
        public string Team { get; set; }
        public int PlayerId { get; set; }
        public int Number { get; set; }
        public int Goals { get; set; }
        public int Penalties { get; set; }
        public int Assist { get; set; }
        public int YellowCards { get; set; }
        public int RedCards { get; set; }
        public int YellowAndRedCards { get; set; }
        public int MissedPenalties { get; set; }
        public int TotalPlayedMinutes { get; set; }
        public int TotalPlayedMatches { get; set; }
    }
}

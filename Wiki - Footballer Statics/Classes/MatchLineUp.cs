using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Wiki___Footballer_Statics.Classes
{
    public class MatchLineUp
    {
        public int MatchId { get; set; }
        public int PlayerId { get; set; }
        public string Team { get; set; }
        public int Number { get; set; }
        public bool IsFirstEleven { get; set; }
        public bool IsSubtituted { get; set; }
        public int? SubstitutionMinute { get; set; }
        public int PlayedMinute
        {
            get
            {
                if (IsFirstEleven&&SubstitutionMinute==0)
                    return 90;
                if (SubstitutionMinute.HasValue && IsFirstEleven)
                {
                    return SubstitutionMinute.Value;

                }
                 if ( IsSubtituted)
                {
                    return 90 - SubstitutionMinute.Value;
                }
                 
                else return 0;
            }
        }
        public Match Match { get; set; }
    }
}

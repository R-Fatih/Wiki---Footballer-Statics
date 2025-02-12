using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Wiki___Footballer_Statics.Helpers
{
  public  class PositionReplacer
    {
        public static string ReplacePosition(string position)
        {
            switch (position)
            {
                case "kaleci":
                    return "[[Kaleci|KL]]";
                case "savunma":
                    return "[[Defans|DF]]";
                case "orta saha":
                case "left winger":
                case "right winger":
                case "wing half":
                    return "[[Orta saha|OS]]";
                case "attacker":
                case "santrfor":
                case "forvet":
                    return "[[Forvet (futbol)|FV]]";
                default:
                    return "-";
            }
        }
    }
}

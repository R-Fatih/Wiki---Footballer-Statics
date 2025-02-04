using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Wiki___Footballer_Statics.Classes
{
    public class Competition
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Country { get; set; }
        public string Season { get; set; }
        public string FullSeasonName { get => Season + " " + Name; }
        public IList<Match> Matches { get; set; }

    }
}

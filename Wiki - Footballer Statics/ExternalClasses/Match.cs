using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Wiki___Footballer_Statics.ExternalClasses
{
    public class D
    {
        public string s { get; set; }
        public int p { get; set; }
        public string st { get; set; }
        public string ht { get; set; }
        public string ft { get; set; }
        public string et { get; set; }
        public string pt { get; set; }
        public int time { get; set; }
    }

    public class Match
    {
        public int seq { get; set; }
        public string home { get; set; }
        public string away { get; set; }
        public D d { get; set; }
        public List<List<object>> h { get; set; }
        public List<List<object>> a { get; set; }
        public List<List<object>> e { get; set; }
        public List<string> sv { get; set; }
    }
}

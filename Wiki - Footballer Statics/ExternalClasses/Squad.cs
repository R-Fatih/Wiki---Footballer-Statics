using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Wiki___Footballer_Statics.ExternalClasses
{

    public class Squad
    {
        public int n { get; set; }
        public object[][] c { get; set; }
        public S[] s { get; set; }
        public string[] a { get; set; }
        public string Team { get; set; }
    }

    public class S
    {
        public string p { get; set; }
        public object[][] s { get; set; }
    }

}

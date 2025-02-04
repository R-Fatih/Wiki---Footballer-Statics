using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Wiki___Footballer_Statics.ExternalClasses
{


    public class WikiDataResponse
    {
        public Head head { get; set; }
        public Results results { get; set; }
    }

    public class Head
    {
        public string[] vars { get; set; }
    }

    public class Results
    {
        public Binding[] bindings { get; set; }
    }

    public class Binding
    {
        public Nationlabel nationLabel { get; set; }
        public Positionlabel positionLabel { get; set; }
        public Birthdate birthDate { get; set; }
        public Formattedname formattedName { get; set; }
        public id id { get; set; }
    }

    public class Nationlabel
    {
        public string xmllang { get; set; }
        public string type { get; set; }
        public string value { get; set; }
    }

    public class Positionlabel
    {
        public string xmllang { get; set; }
        public string type { get; set; }
        public string value { get; set; }
    }

    public class Birthdate
    {
        public string datatype { get; set; }
        public string type { get; set; }
        public DateTime value { get; set; }
    }

    public class Formattedname
    {
        public string type { get; set; }
        public string value { get; set; }
    }
    public class id
    {
        public string type { get; set; }
        public string value { get; set; }
    }
}

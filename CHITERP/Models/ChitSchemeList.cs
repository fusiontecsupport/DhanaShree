using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace CHITERP.Models
{
    public class ChitSchemeList
    {
        public List<ChitScheme> chitscheme { get; set; }

        public List<ChitSchemeCollection> chitschemecollections { get; set; }

        public List<ChitSchemePattern> chitschemepatterns { get; set; }
    }

}
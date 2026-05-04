using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace CHITERP.Models
{
    public class ChitGroupList
    {
        public List<ChitGroup> chitgroup { get; set; }

        public List<ChitGroupRegistered> chitgroupregistered { get; set; }
    }
}
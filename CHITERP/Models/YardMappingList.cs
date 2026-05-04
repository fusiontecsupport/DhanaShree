using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace CHITERP.Models
{
    public class YardMappingList
    {
        

        public List<EmployeeYardMappingDetail> empyardmappings { get; set; }

        public List<SteamerYardMappingDetail> stmyardmappings { get; set; }

    }
}
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace CHITERP.Models
{
    public class CompanyRouteList
    {
        public List<CompanyRoute> cr_masterdata { get; set; }
        public List<CompanyRouteDetail> crd_detaildata { get; set; }
    }
}
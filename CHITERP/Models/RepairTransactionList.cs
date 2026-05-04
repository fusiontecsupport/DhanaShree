using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace CHITERP.Models
{
    public class RepairTransactionList
    {
        public List<RepairTransactionMaster> rmasters { get; set; }

        public List<RepairTransactionDetail> rdetails { get; set; }

        public List<CostFactorMaster> rcostfactor { get; set; }

        public List<RepairTransactionMasterFactor> rmasterfactor { get; set; }
    }
}
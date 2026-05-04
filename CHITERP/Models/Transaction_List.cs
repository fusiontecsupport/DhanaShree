using CHITERP.Models;
using CHITERP.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace CHITERP.Models
{
    public class Transaction_List
    {
        public List<TransactionMaster> masterdata { get; set; }
        public List<TransactionDetail> detaildata { get; set; }
        //public List<TransactionMasterFactor> costfactor { get; set; }
        //public List<TransactionProductSerialDetail> prod_data { get; set; }
        public List<TransactionReceiptDetail> receipt_detail_data { get; set; }
        public List<Tmp_Receipt_Detail> tmp_receipt_detail_data { get; set; }
        //public List<pr_Transaction_Detail_Ctrl_Assgn_Result> detail_view { get; set; }


    }
}
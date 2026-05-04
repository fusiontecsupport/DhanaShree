using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace CHITERP.Models
{
    [Table("TRANSACTIONDETAIL")]
    public class TransactionDetail
    {
        [Key]
        public int TRANDID { get; set; }
        public int TRANMID { get; set; }
        public short TRANDCTYPE { get; set; }
        public int TRANDGREFID { get; set; }
        public int MTRLID { get; set; }
        public int TRANDREFID { get; set; }
        public string TRANDREFNO { get; set; }
        public string TRANDREFNAME { get; set; }
        public decimal TRANDQTY { get; set; }
        public decimal TRANAQTY { get; set; }
        public decimal TRANRQTY { get; set; }
        public decimal TRANDRATE { get; set; }
        public decimal TRANDAMT { get; set; }
        public int TRANDAID { get; set; }
        public int TRANDCID { get; set; }
        public short DISPSTATUS { get; set; }
        public DateTime PRCSDATE { get; set; }
        public int UNITID { get; set; }
        public string TRANDCREFNO { get; set; }
        public decimal TRANDDISCEXPRN { get; set; }
        public decimal TRANDDISCAMT { get; set; }
        public decimal TRANDGAMT { get; set; }
        public decimal TRANDNAMT { get; set; }
        public Nullable<int> ITEMQID { get; set; }
        public string TRAND_ODESC { get; set; }
        public string TRAND_REMKS { get; set; }
        public decimal TRANDCGSTEXPRN { get; set; }
        public decimal TRANDCGSTAMT { get; set; }
        public decimal TRANDSGSTEXPRN { get; set; }
        public decimal TRANDSGSTAMT { get; set; }
        public decimal TRANDIGSTEXPRN { get; set; }
        public decimal TRANDIGSTAMT { get; set; }
        public int HSNID { get; set; }
        public int TRANDLMID { get; set; }
        public decimal TRAND_BCD_EXPRN { get; set; }
        public decimal TRAND_SWS_EXPRN { get; set; }
        public decimal TRAND_INR_VALUE { get; set; }
        public decimal TRAND_EXW_VALUE { get; set; }
        public decimal TRAND_FOB_VALUE { get; set; }
        public decimal TRAND_FRGHT_VALUE { get; set; }
        public decimal TRAND_INS_VALUE { get; set; }
        public decimal TRAND_TOT_COST { get; set; }
        public decimal TRAND_CIF_VALUE { get; set; }
        public decimal TRAND_BCD_VALUE { get; set; }
        public decimal TRAND_SWS_VALUE { get; set; }
        public decimal TRAND_EXCESS_QTY { get; set; }
        public decimal TRAND_SHORT_QTY { get; set; }
        public decimal TRAND_DAMAGE_QTY { get; set; }
        public decimal TRANDARATE { get; set; }
        public decimal TRANDLRATE { get; set; }
        public decimal TRANDAAMT { get; set; }


        public decimal TRAND_PRV_LRATE { get; set; }
        public decimal TRAND_PRV_HIKE_EXPRN { get; set; }
        public decimal TRAND_PRV_SC_RATE { get; set; }
        public decimal TRANDCREXPRN { get; set; }
        public string TRANDINCTYPE { get; set; }


    }
}
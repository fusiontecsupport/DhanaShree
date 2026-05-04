using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace CHITERP.Models
{
    [Table("TRANSACTIONMASTER")]
    public class TransactionMaster
    {
        [Key]
        public int TRANMID { get; set; }
        public int COMPYID { get; set; }
        public int SDPTID { get; set; }
        public int REGSTRID { get; set; }
        public short TRANBTYPE { get; set; }
        public DateTime TRANDATE { get; set; }
        public int TRANNO { get; set; }
        public string TRANDNO { get; set; }
        public int TRANREFID { get; set; }
        public string TRANREFNAME { get; set; }
        public string TRANREFADDR { get; set; }
        public int TRANTID { get; set; }
        public decimal TRANGAMT { get; set; }
        public decimal TRANNAMT { get; set; }
        public string TRANAMTWRDS { get; set; }
        public DateTime TRANSDATE { get; set; }
        public DateTime TRANEDATE { get; set; }
        public Nullable<DateTime> TRANREFDATE { get; set; }
        public string TRANREFNO { get; set; }
        public string TRANDCTHRU { get; set; }
        public string TRANEDESC { get; set; }
        public string TRANDTERM { get; set; }
        public string TRANPTERM { get; set; }
        public string TRANMTERM { get; set; }
        public string TRANIDESC { get; set; }
        public string TRANADESC { get; set; }
        public string TRANPOREFNO { get; set; }
        public string TRANLRREFNO { get; set; }
        public string TRANREMRKS { get; set; }
        public string TRANNARTN { get; set; }
        public int TRANLMID { get; set; }
        public short TRANFTYPE { get; set; }
        public int TRANPCOUNT { get; set; }
        public string CUSRID { get; set; }
        public string LMUSRID { get; set; }
        public short DISPSTATUS { get; set; }
        public DateTime PRCSDATE { get; set; }
        public string TRANEAMTWRDS { get; set; }
        public short QSTATUS { get; set; }
        public Nullable<int> CURNID { get; set; }
        public Nullable<short> TRANQTYPE { get; set; }

        [DisplayFormat(DataFormatString = "{0:n4}", ApplyFormatInEditMode = true)]
        public Nullable<decimal> TRANCRATE { get; set; }
        public Nullable<int> LOCTID { get; set; }
        public Nullable<int> CATEAID { get; set; }
        public Nullable<int> DLOCTID { get; set; }
        public string TRANDELIADDR { get; set; }
        public int TERMMID { get; set; }
        public Nullable<int> TRANANO { get; set; }
        public string TRANADNO { get; set; }
        public Nullable<DateTime> TRANADATE { get; set; }
        public Nullable<int> TRANCHK1 { get; set; }
        public Nullable<int> TRANCHK2 { get; set; }
        public Nullable<int> TRANCHK3 { get; set; }
        public Nullable<int> TRANCHK4 { get; set; }
        public decimal TRANCGSTAMT { get; set; }
        public decimal TRANSGSTAMT { get; set; }
        public decimal TRANIGSTAMT { get; set; }
        public Int16 TRANBCDTYPE { get; set; }
        public Int16 TRANSWSTYPE { get; set; }
        public decimal TRANROAMT { get; set; }
        public string TRANOTHRREFNO { get; set; }
        public Nullable<DateTime> TRANOTHRDATE { get; set; }

        public string TRANRSMNAME { get; set; }
        public string TRANSALENAME { get; set; }
        public string TRANSERVICENAME { get; set; }
        public decimal TRANFOCDISCAMT { get; set; }
        public string TRAN_MCHN_TYPE { get; set; }
        public string TRAN_MCHN_SRLNO { get; set; }
        public string TRAN_MCHN_INK_DESC { get; set; }
        public string TRAN_AMC_MCHN_SRLNO { get; set; }

        public int TRANMODE { get; set; }
        public string TRAN_BANK_NAME { get; set; }
        public string TRAN_BANK_REF_NO { get; set; }
        public Nullable<DateTime> TRAN_BANK_REF_DATE { get; set; }
        public string TRAN_CUST_ADDR1 { get; set; }
        public string TRAN_CUST_ADDR2 { get; set; }
        public string TRAN_CUST_CITY { get; set; }
        public string TRAN_CUST_PINCODE { get; set; }
        public string TRAN_CUST_COUNTRY { get; set; }
        public string TRAN_CUST_STATEDESC { get; set; }
        public string TRAN_DESP_CUST_ADDR1 { get; set; }
        public string TRAN_DESP_CUST_ADDR2 { get; set; }
        public string TRAN_DESP_CUST_CITY { get; set; }
        public string TRAN_DESP_CUST_PINCODE { get; set; }
        public string TRAN_DESP_CUST_COUNTRY { get; set; }
        public string TRAN_DESP_CUST_STATEDESC { get; set; }
        public int TRAN_CUST_STATEID { get; set; }
        public int TRAN_DESP_CUST_STATEID { get; set; }
        public int TRAN_FTYPE { get; set; }
        public int TRAN_SC_STID { get; set; }
        public int TRAN_PFTYPE { get; set; }
        public decimal TRAN_PFEXPRN { get; set; }
        public int TRAN_ADV_TYPE { get; set; }
        public int TRAN_COD_TYPE { get; set; }
        public int TRAN_CREDIT_PERIOD { get; set; }
        public int TRAN_ODA_TYPE { get; set; }
        public int TRAN_TCATEID { get; set; }
        public decimal TRAN_FRGHT_AMT { get; set; }
        public decimal TRAN_ODA_AMT { get; set; }
        public decimal TRAN_COD_AMT { get; set; }
        public int TRAN_LG_MODE { get; set; }
        public int TRAN_LG_STATUS { get; set; }
        public string TRAN_CUST_LOCTDESC { get; set; }
        public string TRAN_DESP_CUST_LOCTDESC { get; set; }
        public int DCATEAID { get; set; }
        public int SCTRANMID { get; set; }
        public int TRAN_RSMID { get; set; }
        public int TRAN_SMID { get; set; }
        public int TRAN_RSERVMID { get; set; }
        public int TRAN_SERVMID { get; set; }
        public int TRAN_TO_REFID { get; set; }
        public string TRAN_TO_REFNAME { get; set; }
        public int TRAN_LINKID { get; set; }
        public string TRAN_LINK_NO { get; set; }
        public Nullable<DateTime> TRAN_LINK_DATE { get; set; }
        public int TRAN_CUSTGID { get; set; }
        public string TRAN_LUT_NO { get; set; }
        public int TRAN_PF_INCL_TYPE { get; set; }
        public decimal TRAN_PF_INCL_EXPRN { get; set; }
        public Nullable<int> LinkedOfficeID { get; set; }


    }
}
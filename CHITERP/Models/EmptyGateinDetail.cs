using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace CHITERP.Models
{
    [Table("EMPTY_GATEINDETAIL")]
    public class EmptyGateinDetail
    {
		[Key]
		public int GIDID { get; set; }

		public int COMPYID { get; set; }

		public int SDPTID { get; set; }

		public DateTime GIDATE { get; set; }

		public DateTime GITIME { get; set; }

		public Nullable<DateTime> GICCTLDATE { get; set; }

		public Nullable<DateTime> GICCTLTIME { get; set; }

		public int GINO { get; set; }

		public string GIDNO { get; set; }

		public Int16 GIVHLTYPE { get; set; }

		public int TRNSPRTID { get; set; }

		public string TRNSPRTNAME { get; set; }

		public string VHLNO { get; set; }

		public string DRVNAME { get; set; }

		public string GPREFNO { get; set; }

		public int IMPRTID { get; set; }

		public string IMPRTNAME { get; set; }

		public int STMRID { get; set; }

		public string STMRNAME { get; set; }

		public int CONTNRTID { get; set; }

		public int CONTNRID { get; set; }

		public int CONTNRSID { get; set; }

		public string CONTNRNO { get; set; }

		public string LPSEALNO { get; set; }

		public string CSEALNO { get; set; }

		public int YRDID { get; set; }

		public int VSLID { get; set; }

		public string VSLNAME { get; set; }

		public string VOYNO { get; set; }

		public int PRDTGID { get; set; }

		public string PRDTDESC { get; set; }

		public int UNITID { get; set; }

		public decimal GPWGHT { get; set; }

		public string IGMNO { get; set; }

		public string GPLNO { get; set; }

		public Int16 GPWTYPE { get; set; }

		public Int16 GPSTYPE { get; set; }

		public Int16 GPETYPE { get; set; }

		public int BOEDID { get; set; }

		public int INVDID { get; set; }

		public int RGIDID { get; set; }

		public string GIREMKRS { get; set; }

		//public int ROWID { get; set; }

		public int SLOTID { get; set; }

		public decimal GPEAMT { get; set; }

		public int CHAID { get; set; }

		public string CHANAME { get; set; }

		public string AVHLNO { get; set; }

		public decimal GPNOP { get; set; }

		public int PRDTTID { get; set; }

		public string GIISOCODE { get; set; }

		public string GIDMGDESC { get; set; }

		public int NGIDID { get; set; }

		public decimal GPNWGHT { get; set; }

		public decimal GPGWGHT { get; set; }

		public decimal GPCBM { get; set; }

		public decimal GPLENGTH { get; set; }

		public decimal GPWIDTH { get; set; }

		public decimal GPHEIGHT { get; set; }

		public string GPBLNO { get; set; }

		public decimal GPAAMT { get; set; }

		public int CLNTID { get; set; }

		public string CLNTNAME { get; set; }

		public string SHPRNAME { get; set; }

		public string GPPLCNAME { get; set; }

		public decimal GPTWGHT { get; set; }

		public int CONTNRFID { get; set; }

		public int CONDTNID { get; set; }

		public int GRADEID { get; set; }

		public int CNTNRSID { get; set; }

		public Nullable<DateTime> GISDMDATE { get; set; }

		public string GISDMDESC { get; set; }

		public Nullable<DateTime> GISAVDATE { get; set; }

		public string GISAVDESC { get; set; }

		public Int16 EGPSCITYPE { get; set; }

		public Int16 EGPVTYPE { get; set; }

		public Nullable<DateTime> EGPVDATE { get; set; }

		public int GIESTTID { get; set; }

		public int GILTYPE { get; set; }

		public string GISENDERNAME { get; set; }

		public string GIENTRYNO { get; set; }

		public string GIMATERIAL { get; set; }

		public string GIMEASURE { get; set; }

		public string GIUNIT { get; set; }

		public string GICSC { get; set; }

		public string GILESSOR { get; set; }

		public string GILASTLOCTCODE { get; set; }

		public Nullable<DateTime> GILASTLOCTDATE { get; set; }

		public int LCONDTNID { get; set; }

		public int GAVINO { get; set; }

		public int LCNTNRSID { get; set; }

		public Nullable<DateTime> LGISAVDATE { get; set; }

		public string LGISAVDESC { get; set; }		

		public string CUSRID { get; set; }

		public string LMUSRID { get; set; }

		public Int16 DISPSTATUS { get; set; }

		public Nullable<DateTime> PRCSDATE { get; set; }
		

	}
}
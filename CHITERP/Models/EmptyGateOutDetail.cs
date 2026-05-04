using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace CHITERP.Models
{

    [Table("EMPTY_GATEOUTDETAIL")]
    public class EmptyGateOutDetail
    {
        [Key]
        public int GODID { get; set; }

        public int COMPYID { get; set; }

        public int SDPTID { get; set; }

        public int REGSTRID { get; set; }

        public DateTime GODATE { get; set; }

        public DateTime GOTIME { get; set; }

        public int GONO { get; set; }

        public string GODNO { get; set; }

        public int GIDID { get; set; }

        public int TRANDID { get; set; }

        public string VHLNO { get; set; }

        public string GDRVNAME { get; set; }

        public string CHASNAME { get; set; }

        public Int16 GOBTYPE { get; set; }

        public int CHAID { get; set; }

        public string LSEALNO { get; set; }

        public string SSEALNO { get; set; }

        public decimal GOQTY { get; set; }

        public int OTDID { get; set; }

        public string OTDNO { get; set; }

        public int CONTNRFID { get; set; }

        public int BOOKID { get; set; }

        public string GOPLCNAME { get; set; }

        public string GOBKGNO { get; set; }

        public string GOEGMNO { get; set; }

        public int OVSLID { get; set; }

        public string OVSLNAME { get; set; }

        public string OVOYNO { get; set; }

        public string GOREMRKS { get; set; }

        public string OTRNSPRTNAME { get; set; }

        public int GBKDID { get; set; }

        public string GOFORM13NO { get; set; }

        public string GOPODDETL { get; set; }

        public string GOISOCODE { get; set; }

        public Nullable<DateTime> GOSALEDATE { get; set; }

        public Nullable<DateTime> GOSAILDATE { get; set; }

        public int GOASIS_TYPE { get; set; }

        public string CUSRID { get; set; }

        public string LMUSRID { get; set; }

        public Int16 DISPSTATUS { get; set; }

        public DateTime PRCSDATE { get; set; }

    }
}
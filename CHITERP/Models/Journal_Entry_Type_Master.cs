using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace CHITERP.Models
{
    [Table("JOURNAL_ENTRY_TYPE_MASTER")]
    public class Journal_Entry_Type_Master
    {
        [Key]
        public int JETID { get; set; }
        public string JETDESC { get; set; }
        public string JETCODE { get; set; }
        public string CUSRID { get; set; }
        public string LMUSRID { get; set; }
        public short DISPSTATUS { get; set; }
        public DateTime PRCSDATE { get; set; }
    }
}
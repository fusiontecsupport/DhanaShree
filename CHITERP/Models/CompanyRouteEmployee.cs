using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace CHITERP.Models
{
    [Table("CompanyRouteEmployee")]
    public class CompanyRouteEmployee
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.None)]
        public int RouteEmployeeID { get; set; }

        public int RouteID { get; set; }

        public int EmployeeID { get; set; }

        public DateTime? StartDate { get; set; }

        public DateTime? EndDate { get; set; }

        public int StatusTypeID { get; set; }
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace CHITERP.Models
{
    public class CategoryList
    {
        public List<CategoryMaster> catemaster { get; set; }
        public List<Category_Address_Details> cateadddetails { get; set; }

    }
}
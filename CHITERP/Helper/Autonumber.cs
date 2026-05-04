using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using CHITERP.Models;

namespace CHITERP.Helper
{
    public class Autonumber
    {
        public static string autonum(String table_name, String table_fld, String record_condn)
        {

            String temp = "";

            using (var context = new AppDbContext())
            {
                temp = "1";

                var s = "SELECT MAX(" + table_fld + ") as " + table_fld + " from " + table_name + " where " + record_condn + "";

                var autonum = context.Database.SqlQuery<Nullable<Int32>>("SELECT   isnull(MAX(" + table_fld + "),0) as " + table_fld + " from " + table_name + " where " + record_condn).ToList();
                if (autonum[0] != null)
                {

                    //  var autonumber = context.Database.SqlQuery<Int32>("SELECT MAX(" + table_fld + ") as " + table_fld + " from " + table_name + " where " + record_condn).ToList();
                    temp = (autonum[0] + 1).ToString();
                }
                else
                {
                    return temp;
                }


            }


            return temp;
        }
        public static string transactiomaster_autonum(String table_name, String table_fld, String record_condn)
        {

            String temp = "";

            using (var context = new AppDbContext())
            {
                temp = "1";
                var autonum = context.Database.SqlQuery<Nullable<Int32>>("SELECT MAX(" + table_fld + ")as " + table_fld + " from " + table_name + " where " + record_condn).ToList();
                if (autonum[0] != null)
                {
                    temp = (autonum[0] + 1).ToString();
                }
                else
                {
                    return temp;
                }
            }


            return temp;
        }
    }
}
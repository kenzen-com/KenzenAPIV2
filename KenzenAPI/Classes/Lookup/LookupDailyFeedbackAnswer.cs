using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Threading.Tasks;

namespace KenzenAPI.Classes.Lookup
{
    public class DailyFeedbackAnswer : LookupBase
    {
        public DailyFeedbackAnswer()
        {
            this.TableName = "LookupDailyFeedbackAnswer";
        }
        public override ProcessResult FetchAll(IConfiguration Config)
        {
            ProcessResult oPR = new ProcessResult();
            try
            {
                List<DailyFeedbackAnswer> oOut = new List<DailyFeedbackAnswer>();
                SqlCommand cmd = new SqlCommand("SELECT * FROM " + this.TableName);
                cmd.Connection.Open();
                using (cmd.Connection)
                {
                    SqlDataReader dr = cmd.ExecuteReader();
                    while (dr.Read())
                    {
                        DailyFeedbackAnswer a = new DailyFeedbackAnswer();
                        a.ID = Convert.ToInt32(dr["ID"]);
                        a.Name = dr["Name"].ToString();
                        oOut.Add(a);
                    }
                }

                oPR.ObjectProcessed = oOut;
            }
            catch (Exception exc)
            {
                oPR.Exception = exc;
            }
            return oPR;
        }
    }
}

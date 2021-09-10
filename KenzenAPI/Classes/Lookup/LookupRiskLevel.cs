using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Threading.Tasks;

namespace KenzenAPI.Classes.Lookup
{
    public class RiskLevel : LookupBase
    {
        public RiskLevel()
        {
            this.TableName = "LookupRiskLevel";
        }
        public override ProcessResult FetchAll(IConfiguration Config)
        {
            ProcessResult oPR = new ProcessResult();
            try
            {
                List<RiskLevel> oOut = new List<RiskLevel>();
                SqlCommand cmd = new SqlCommand("SELECT * FROM " + this.TableName);
                cmd.Connection.Open();
                using (cmd.Connection)
                {
                    SqlDataReader dr = cmd.ExecuteReader();
                    while (dr.Read())
                    {
                        RiskLevel a = new RiskLevel();
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

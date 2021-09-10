using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using Microsoft.Extensions.Configuration;
using System.Threading.Tasks;

namespace KenzenAPI.Classes.Lookup
{
    public class LookupDevice : LookupBase
    {
        public LookupDevice()
        {
            this.TableName = "LookupDevice";
        }
        public override ProcessResult FetchAll(IConfiguration Config)
        {
            ProcessResult oPR = new ProcessResult();
            try
            {
                List<LookupDevice> oOut = new List<LookupDevice>();
                SqlCommand cmd = new SqlCommand("SELECT * FROM " + this.TableName);
                SqlDataReader dr = cmd.ExecuteReader();
                while (dr.Read())
                {
                    LookupDevice a = new LookupDevice();
                    a.ID = Convert.ToInt32(dr["ID"]);
                    a.Name = dr["Name"].ToString();
                    oOut.Add(a);
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

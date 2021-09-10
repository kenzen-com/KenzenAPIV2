using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Threading.Tasks;

namespace KenzenAPI.Classes.Lookup
{
    public class Role : LookupBase
    {
        public Role()
        {
            this.TableName = "LookupRole";
        }
        public override ProcessResult FetchAll(IConfiguration Config)
        {
            ProcessResult oPR = new ProcessResult();
            try
            {
                List<Role> oOut = new List<Role>();
                SqlCommand cmd = new SqlCommand("SELECT * FROM " + this.TableName);
                cmd.Connection = new SqlConnection(Client.GetCnxnString(0, Config));
                cmd.Connection.Open();
                using (cmd.Connection)
                {
                    SqlDataReader dr = cmd.ExecuteReader();
                    while (dr.Read())
                    {
                        Role a = new Role();
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

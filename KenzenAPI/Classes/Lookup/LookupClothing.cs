using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;
namespace KenzenAPI.Classes.Lookup
{
    public class Clothing : LookupBase
    {
        public Clothing()
        {
            this.TableName = "LookupClothing";
        }
        public override ProcessResult FetchAll(IConfiguration Config)
        {
            ProcessResult oPR = new ProcessResult();
            try
            {
                List<Clothing> oOut = new List<Clothing>();
                SqlCommand cmd = new SqlCommand("SELECT * FROM " + this.TableName);
                cmd.Connection = new SqlConnection(Client.GetCnxnString(0, Config));
                cmd.Connection.Open();
                using (cmd.Connection)
                {
                    SqlDataReader dr = cmd.ExecuteReader();
                    while (dr.Read())
                    {
                        Clothing a = new Clothing();
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

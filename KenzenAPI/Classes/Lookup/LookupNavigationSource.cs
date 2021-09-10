using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;
namespace KenzenAPI.Classes.Lookup
{
    public class NavigationSource : LookupBase
    {
        public NavigationSource()
        {
            this.TableName = "LookupNavigationSource";
        }
        public override ProcessResult FetchAll(IConfiguration Config)
        {
            ProcessResult oPR = new ProcessResult();
            try
            {
                List<NavigationSource> oOut = new List<NavigationSource>();
                SqlCommand cmd = new SqlCommand("SELECT * FROM " + this.TableName);
                cmd.Connection.Open();
                using (cmd.Connection)
                {
                    SqlDataReader dr = cmd.ExecuteReader();
                    while (dr.Read())
                    {
                        NavigationSource a = new NavigationSource();
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

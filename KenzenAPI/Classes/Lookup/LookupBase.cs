using Microsoft.Extensions.Configuration;
using Newtonsoft.Json;
using Serilog;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Threading.Tasks;

namespace KenzenAPI.Classes.Lookup
{
    public abstract class LookupBase
    {
        public int ID { get; set; }
        public string Name { get; set; }
        [JsonIgnore]
        public string TableName { get; set; }

        public virtual ProcessResult FetchByID(int ID, IConfiguration Config)
        {
            ProcessResult oPR = new ProcessResult();
            try
            {
                SqlCommand cmd = new SqlCommand("SELECT * FROM " + this.TableName + "WHERE ID = @ID");
                cmd.Connection = new SqlConnection(Client.GetCnxnString(0, Config));
                cmd.Parameters.AddWithValue("@ID", SqlDbType.Int).Value = ID;
                SqlDataReader dr = cmd.ExecuteReader();
                while (dr.Read())
                {
                    this.ID = Convert.ToInt32(dr["ID"]);
                    this.Name = dr["Name"].ToString();
                }

                oPR.ObjectProcessed = this;
            }
            catch (Exception exc)
            {
                oPR.Exception = exc;
            }
            return oPR;
        }
        public virtual ProcessResult Delete(int ID, IConfiguration Config)
        {
            ProcessResult oPR = new ProcessResult();
            try
            {
                SqlCommand cmd = new SqlCommand("DELETE FROM " + this.TableName + "WHERE ID = @ID");
                cmd.Connection = new SqlConnection(Client.GetCnxnString(0, Config));
                cmd.Parameters.AddWithValue("@ID", SqlDbType.Int).Value = ID;
                cmd.ExecuteNonQuery();
                oPR.ObjectProcessed = ID;
                oPR.Result = "Deleted";
            }
            catch (Exception exc)
            {
                oPR.Exception = exc;
            }
            return oPR;
        }
        public virtual ProcessResult Save(IConfiguration Config)
        {
            ProcessResult oPR = new ProcessResult();
            try
            {
                SqlCommand cmd = new SqlCommand("INSERT INTO " + this.TableName + "(ID, Name) VALUES (@ID, @Name)");
                cmd.Connection = new SqlConnection(Client.GetCnxnString(0, Config));
                cmd.Parameters.AddWithValue("@ID", SqlDbType.Int).Value = this.ID;
                cmd.Parameters.AddWithValue("@Name", SqlDbType.Int).Value = this.Name;
                cmd.ExecuteNonQuery();
                oPR.ObjectProcessed = this;
                oPR.Result = "Saved";
            }
            catch (Exception exc)
            {
                oPR.Exception = exc;
            }
            return oPR;
        }
        public abstract ProcessResult FetchAll(IConfiguration Config);
    }
}

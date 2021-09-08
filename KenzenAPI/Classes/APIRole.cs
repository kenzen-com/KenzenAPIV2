
using System;
using System.Data;
using System.Data.SqlClient;
using System.Collections.Generic;
using System.Reflection;
using System.Runtime.Serialization;
using System.Configuration;
using System.Runtime.CompilerServices;
using KenzenAPI.Classes;
using Serilog;
using Microsoft.Extensions.Configuration;

namespace KenzenAPI.DataClasses
{

    public class APIRoleCollection : Dictionary<int, APIRole>
    {
        public ILogger Logger;
        public IConfiguration Config;

        #region Constructors

        public APIRoleCollection(){}
        public APIRoleCollection(ILogger logger, IConfiguration config)
        {
            Logger = logger;
            Config = config;

            SqlConnection Cnxn = new SqlConnection(Config["CnxnString"]);
            try
            {

                APIRole oAPIRole = new APIRole(null, null);
                SqlCommand cmd = new SqlCommand(oAPIRole.SQLFetchAllString(), Cnxn);

                Cnxn.Open();
                SqlDataReader dr = cmd.ExecuteReader();
                while (dr.Read())
                {
                    oAPIRole = new APIRole(Logger, Config);
                    oAPIRole.Name = dr["Name"] == DBNull.Value ? "" : dr["Name"].ToString().Trim();
                    oAPIRole.ID = dr["ID"] == DBNull.Value ? 0 : Convert.ToInt32(dr["ID"]);
                    if (!this.ContainsKey(oAPIRole.ID))
                        this.Add(oAPIRole.ID, oAPIRole);
                }

                dr.Close();
                Cnxn.Close();
            }
            catch (Exception Exc)
            {
                Log.LogErr("APIRoleCollectionConstructor", Exc.Message, Config["LogPath"]);
            }
            finally
            {
                if (Cnxn.State == ConnectionState.Open) Cnxn.Close();
            }
        }

        #endregion Constructors

    }



    public class APIRole : DataClassBase
    {

        #region Vars


        string _Name;

        #endregion Vars

        #region Get/Sets
        public string Name
        {
            get { return (_Name); }
            set { _Name = value; }
        }

        #endregion Get/Sets

        #region Constructors

        public APIRole(ILogger logger, IConfiguration config)
        {
            Logger = logger;
            Config = config;
            TableName = "APIRoles";
        }

        public APIRole(int ID, ILogger logger, IConfiguration config) : this(logger, config)
        {
            string CnxnString = Config["CnxnString"];
            string LogPath = Config["LogPath"];

            SqlConnection Cnxn = new SqlConnection(CnxnString);
            try
            {

                SqlCommand cmd = new SqlCommand("SELECT * FROM " + this.TableName + " WHERE ID = @ID", Cnxn);

                cmd.Parameters.Add(new SqlParameter("@ID", SqlDbType.Int));
                cmd.Parameters["@ID"].Value = ID;

                Cnxn.Open();
                SqlDataReader dr = cmd.ExecuteReader();
                while (dr.Read())
                {

                    this.Name = dr["Name"] == DBNull.Value ? "" : dr["Name"].ToString().Trim();
                    this.ID = dr["ID"] == DBNull.Value ? 0 : Convert.ToInt32(dr["ID"]);
                }

                dr.Close();
                Cnxn.Close();
            }
            catch (Exception Exc)
            {
                Log.LogErr("APIRoleConstructor", Exc.Message, LogPath);
            }
            finally
            {
                if (Cnxn.State == ConnectionState.Open) Cnxn.Close();
            }


        }

        #endregion Constructors

        #region Save
        public ProcessResult Save()
        {
            string CnxnString = Config["CnxnString"];
            string LogPath = Config["LogPath"];

            ProcessResult oPR = new ProcessResult();
            SqlConnection Cnxn = new SqlConnection(CnxnString);
            try
            {

                SqlCommand cmd = new SqlCommand("INSERT INTO [" + this.SchemaName + "].[" + this.TableName + "] (ID, Name) VALUES (@ID, @Name)  SET @IDOut = @@IDENTITY", Cnxn);

                #region Parameters
                // parameters for APIRoles
                cmd.Parameters.Add(new SqlParameter("@Name", SqlDbType.VarChar, 50));
                cmd.Parameters["@Name"].Value = this.Name ?? "";

                cmd.Parameters.Add(new SqlParameter("@ID", SqlDbType.Int));
                cmd.Parameters["@ID"].Value = this.ID;

                // assign output param
                cmd.Parameters.Add(new SqlParameter("@IDOut", SqlDbType.Int));
                cmd.Parameters["@IDOut"].Direction = ParameterDirection.Output;

                #endregion Parameters

                Cnxn.Open();
                cmd.ExecuteNonQuery();
                Cnxn.Close();

                int iID = Convert.ToInt32(cmd.Parameters["@IDOut"].Value);
                this.ID = iID;

                oPR.ObjectProcessed = this;
                oPR.Result += "Saved";
                return (oPR);

            }
            catch (Exception Exc)
            {
                Log.LogErr("APIRoleSave", Exc.Message, LogPath);

                oPR.Exception = Exc;
                oPR.Result += "Error";
                return (oPR);
            }
            finally
            {
                if (Cnxn.State == ConnectionState.Open) Cnxn.Close();
            }
        }
        #endregion Save

        #region Delete


        public bool Delete()
        {
            string CnxnString = Config["CnxnString"];
            string LogPath = Config["LogPath"];

            SqlConnection Cnxn = new SqlConnection(CnxnString);
            try
            {

                SqlCommand cmd = new SqlCommand("DELETE FROM [" + this.SchemaName + "].[" + this.TableName + "] WHERE ID = @ID", Cnxn);

                cmd.Parameters.Add(new SqlParameter("@ID", SqlDbType.Int));
                cmd.Parameters["@ID"].Value = ID;

                Cnxn.Open();
                cmd.ExecuteNonQuery();
                Cnxn.Close();
                return (true);
            }
            catch (Exception Exc)
            {
                Log.LogErr("APIRoleDelete", Exc.Message, LogPath);
                return (false);
            }
            finally
            {
                if (Cnxn.State == ConnectionState.Open) Cnxn.Close();
            }


        }
        #endregion Delete

        public string SQLFetchAllString()
        {
            string sSQL = "SELECT * FROM [" + this.SchemaName + "].[" + this.TableName + "]";
            return (sSQL.Trim());
        }
    }
}
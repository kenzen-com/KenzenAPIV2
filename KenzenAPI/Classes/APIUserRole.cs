using System;
using System.Data;
using System.Data.SqlClient;
using System.Collections.Generic;
using System.Reflection;
using System.Runtime.Serialization;
using System.Configuration;
using KenzenAPI.Classes;
using Serilog;
using Microsoft.Extensions.Configuration;

namespace KenzenAPI.DataClasses
{

    public class APIUserRoleCollection : Dictionary<int, APIUserRole>
    {
        public ILogger Logger;
        public IConfiguration Config;

        #region Constructors
        public APIUserRoleCollection() { }
        public APIUserRoleCollection(ILogger logger, IConfiguration config)
        {
            Logger = logger;
            Config = config;

            SqlConnection Cnxn = new SqlConnection(Config["CnxnString"]);
            try
            {

                APIUserRole oAPIUserRole = new APIUserRole(null, null);
                SqlCommand cmd = new SqlCommand("SELECT * FROM [" + oAPIUserRole.SchemaName + "].[" + oAPIUserRole.TableName + "]", Cnxn);

                Cnxn.Open();
                SqlDataReader dr = cmd.ExecuteReader();
                while (dr.Read())
                {
                    oAPIUserRole = new APIUserRole(Logger, Config);
                    oAPIUserRole.UserID = dr["UserID"] == DBNull.Value ? 0 : Convert.ToInt32(dr["UserID"]);
                    oAPIUserRole.ClientID = dr["ClientID"] == DBNull.Value ? 0 : Convert.ToInt32(dr["ClientID"]);
                    oAPIUserRole.ID = dr["ID"] == DBNull.Value ? 0 : Convert.ToInt32(dr["ID"]);
                    oAPIUserRole.RoleID = dr["RoleID"] == DBNull.Value ? 0 : Convert.ToInt32(dr["RoleID"]);
                    oAPIUserRole.CreatedDate = dr["CreatedDate"] == DBNull.Value ? "" : Convert.ToDateTime(dr["CreatedDate"]).ToUniversalTime().ToString("o").Trim();
                    if (!this.ContainsKey(oAPIUserRole.ID))
                        this.Add(oAPIUserRole.ID, oAPIUserRole);
                }

                dr.Close();
                Cnxn.Close();
            }
            catch (Exception Exc)
            {
                Log.LogErr("APIUserRoleCollectionConstructor", Exc.Message, Config["LogPath"]);
            }
            finally
            {
                if (Cnxn.State == ConnectionState.Open) Cnxn.Close();
            }
        }

        #endregion Constructors

    }



    public class APIUserRole : DataClassBase
    {

        #region Vars

        int _UserID;

        string _CreatedDate;
        int _RoleID;
        int _ClientID;

        #endregion Vars

        #region Get/Sets

        public int UserID
        {
            get { return (_UserID); }
            set { _UserID = value; }
        }

        public string CreatedDate
        {
            get { return (Convert.ToDateTime(_CreatedDate).ToString("o")); }
            set { _CreatedDate = value; }
        }

        public int RoleID
        {
            get { return (_RoleID); }
            set { _RoleID = value; }
        }

        public int ClientID
        {
            get { return (_ClientID); }
            set { _ClientID = value; }
        }

        #endregion Get/Sets

        #region Constructors

        public APIUserRole(ILogger logger, IConfiguration config)
        {
            Logger = logger;
            Config = config;
            TableName = "APIUserRoles";
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

                SqlCommand cmd = new SqlCommand("INSERT INTO [" + this.SchemaName + "].[" + this.TableName + "] (ID, UserID, RoleID, ClientID)" +
                    " VALUES (@ID, @UserID, @RoleID, @ClientID)  SET @IDOut = @@IDENTITY", Cnxn);

                #region Parameters
                // parameters for APIUserRoles
                cmd.Parameters.Add(new SqlParameter("@UserID", SqlDbType.Int));
                cmd.Parameters["@UserID"].Value = this.UserID;

                cmd.Parameters.Add(new SqlParameter("@ClientID", SqlDbType.Int));
                cmd.Parameters["@ClientID"].Value = this.ClientID;

                cmd.Parameters.Add(new SqlParameter("@ID", SqlDbType.Int));
                cmd.Parameters["@ID"].Value = this.ID;

                cmd.Parameters.Add(new SqlParameter("@RoleID", SqlDbType.Int));
                cmd.Parameters["@RoleID"].Value = this.RoleID;

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
                Log.LogErr("APIUserRoleSave", Exc.Message, LogPath);

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

            ProcessResult oPR = new ProcessResult();
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
                Log.LogErr("APIUserRoleDelete", Exc.Message, LogPath);
                return (false);
            }
            finally
            {
                if (Cnxn.State == ConnectionState.Open) Cnxn.Close();
            }


        }
        #endregion Delete

    }
}
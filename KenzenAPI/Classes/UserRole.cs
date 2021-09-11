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

    public class UserRoleCollection : Dictionary<int, UserRole>
    {
        public ILogger Logger;
        public IConfiguration Config;

        #region Constructors
        public UserRoleCollection() { }
        public UserRoleCollection(ILogger logger, IConfiguration config)
        {
            Logger = logger;
            Config = config;

            SqlConnection Cnxn = new SqlConnection(Client.GetCnxnString(0, Config));
            try
            {

                UserRole oUserRole = new UserRole(null, null);
                SqlCommand cmd = new SqlCommand("SELECT * FROM " + oUserRole.TableName, Cnxn);
                cmd.Connection = new SqlConnection(Client.GetCnxnString(0, Config));

                Cnxn.Open();
                SqlDataReader dr = cmd.ExecuteReader();
                while (dr.Read())
                {
                    oUserRole = new UserRole(Logger, Config);
                    oUserRole.UserID = dr["UserID"] == DBNull.Value ? 0 : Convert.ToInt32(dr["UserID"]);
                    oUserRole.ClientID = dr["ClientID"] == DBNull.Value ? 0 : Convert.ToInt32(dr["ClientID"]);
                    oUserRole.ID = dr["ID"] == DBNull.Value ? 0 : Convert.ToInt32(dr["ID"]);
                    oUserRole.RoleID = dr["RoleID"] == DBNull.Value ? 0 : Convert.ToInt32(dr["RoleID"]);
                    oUserRole.UTC = dr["UTC"] == DBNull.Value ? "" : Convert.ToDateTime(dr["UTC"]).ToUniversalTime().ToString("o").Trim();
                    if (!this.ContainsKey(oUserRole.ID))
                        this.Add(oUserRole.ID, oUserRole);
                }

                dr.Close();
                Cnxn.Close();
            }
            catch (Exception Exc)
            {
                Log.LogErr("UserRoleCollectionConstructor", Exc.Message, Config["LogPath"]);
            }
            finally
            {
                if (Cnxn.State == ConnectionState.Open) Cnxn.Close();
            }
        }

        #endregion Constructors

    }



    public class UserRole : DataClassBase
    {

        #region Vars

        int _UserID;

        string _UTC;
        int _RoleID;
        int _ClientID;

        #endregion Vars

        #region Get/Sets

        public int UserID
        {
            get { return (_UserID); }
            set { _UserID = value; }
        }

        public string UTC
        {
            get { return (Convert.ToDateTime(_UTC).ToString("o")); }
            set { _UTC = value; }
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

        public UserRole(ILogger logger, IConfiguration config)
        {
            Logger = logger;
            Config = config;
            TableName = "UserRoles";
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

                SqlCommand cmd = new SqlCommand("INSERT INTO " + this.TableName + " (ID, UserID, RoleID, ClientID)" +
                    " VALUES (@ID, @UserID, @RoleID, @ClientID)  SET @IDOut = @@IDENTITY", Cnxn);

                #region Parameters
                // parameters for UserRoles
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
                Log.LogErr("UserRoleSave", Exc.Message, LogPath);

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

                SqlCommand cmd = new SqlCommand("DELETE FROM " + this.TableName + " WHERE ID = @ID", Cnxn);

                cmd.Parameters.Add(new SqlParameter("@ID", SqlDbType.Int));
                cmd.Parameters["@ID"].Value = ID;

                Cnxn.Open();
                cmd.ExecuteNonQuery();
                Cnxn.Close();
                return (true);
            }
            catch (Exception Exc)
            {
                Log.LogErr("UserRoleDelete", Exc.Message, LogPath);
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